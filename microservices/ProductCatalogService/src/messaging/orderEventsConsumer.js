const amqp = require('amqplib');
const giftService = require('../services/giftService');
const ProcessedEvent = require('../models/ProcessedEvent');

const EXCHANGE = process.env.RABBITMQ_EXCHANGE || 'order.events';
const RESERVED_QUEUE = process.env.RABBITMQ_CATALOG_RESERVED_QUEUE || 'product-catalog.inventory-reserved';
const FAIL_QUEUE = process.env.RABBITMQ_CATALOG_FAIL_QUEUE || 'product-catalog.purchase-failed';

function getAmqpUrl() {
  const host = process.env.RABBITMQ_HOST || 'localhost';
  const port = process.env.RABBITMQ_PORT || '5672';
  const user = process.env.RABBITMQ_USER || 'guest';
  const password = process.env.RABBITMQ_PASSWORD || 'guest';
  const vhost = process.env.RABBITMQ_VHOST || '/';
  const encodedVhost = encodeURIComponent(vhost);
  return `amqp://${user}:${password}@${host}:${port}/${encodedVhost}`;
}

async function ensureNotProcessed(eventId, payload, eventType) {
  const existing = await ProcessedEvent.findOne({ eventId }).lean();
  if (existing) {
    console.log(`DUPLICATE_EVENT eventType=${eventType} eventId=${eventId} giftId=${payload.giftId}`);
    return false;
  }

  await ProcessedEvent.create({
    eventId,
    correlationId: payload.correlationId,
    sagaId: payload.sagaId,
    eventType,
    giftId: payload.giftId
  });

  return true;
}

async function startOrderEventsConsumer() {
  const connection = await amqp.connect(getAmqpUrl());
  const channel = await connection.createChannel();

  await channel.assertExchange(EXCHANGE, 'topic', { durable: true });
  await channel.assertQueue(RESERVED_QUEUE, { durable: true });
  await channel.assertQueue(FAIL_QUEUE, { durable: true });
  await channel.bindQueue(RESERVED_QUEUE, EXCHANGE, 'order.events.inventory-reserved');
  await channel.bindQueue(FAIL_QUEUE, EXCHANGE, 'order.events.purchase-failed');
  channel.prefetch(20);

  channel.consume(RESERVED_QUEUE, async msg => {
    if (!msg) return;
    try {
      const payload = JSON.parse(msg.content.toString());
      const shouldProcess = await ensureNotProcessed(payload.eventId, payload, 'inventory-reserved');
      if (!shouldProcess) {
        channel.ack(msg);
        return;
      }

      await giftService.incrementBuyers(Number(payload.giftId));
      channel.ack(msg);
    } catch (err) {
      console.error('Catalog consumer inventory-reserved error:', err.message);
      channel.nack(msg, false, true);
    }
  });

  channel.consume(FAIL_QUEUE, async msg => {
    if (!msg) return;
    try {
      const payload = JSON.parse(msg.content.toString());
      const shouldProcess = await ensureNotProcessed(payload.eventId, payload, 'purchase-failed');
      if (!shouldProcess) {
        channel.ack(msg);
        return;
      }

      await giftService.decrementBuyers(Number(payload.giftId));
      channel.ack(msg);
    } catch (err) {
      console.error('Catalog consumer purchase-failed error:', err.message);
      channel.nack(msg, false, true);
    }
  });

  console.log('ProductCatalogService RabbitMQ consumers started');
  return { connection, channel };
}

module.exports = { startOrderEventsConsumer };