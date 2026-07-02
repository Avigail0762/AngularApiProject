const amqp = require('amqplib');
const axios = require('axios');
const emailService = require('../services/emailService');

const EXCHANGE = process.env.RABBITMQ_EXCHANGE || 'order.events';
const QUEUE = process.env.RABBITMQ_NOTIFICATION_QUEUE || 'notification.order-finalized';
const ORDER_URL = () => process.env.ORDER_URL;
const INTERNAL_HEADERS = () => ({ 'x-internal-secret': process.env.INTERNAL_SECRET });

function getAmqpUrl() {
  const host = process.env.RABBITMQ_HOST || 'localhost';
  const port = process.env.RABBITMQ_PORT || '5672';
  const user = process.env.RABBITMQ_USER || 'guest';
  const password = process.env.RABBITMQ_PASSWORD || 'guest';
  const vhost = process.env.RABBITMQ_VHOST || '/';
  const encodedVhost = encodeURIComponent(vhost);
  return `amqp://${user}:${password}@${host}:${port}/${encodedVhost}`;
}

async function startOrderFinalizedConsumer() {
  const connection = await amqp.connect(getAmqpUrl());
  const channel = await connection.createChannel();

  await channel.assertExchange(EXCHANGE, 'topic', { durable: true });
  await channel.assertQueue(QUEUE, { durable: true });
  await channel.bindQueue(QUEUE, EXCHANGE, 'order.events.order-finalized');
  channel.prefetch(20);

  channel.consume(QUEUE, async msg => {
    if (!msg) return;
    try {
      const payload = JSON.parse(msg.content.toString());
      const userRes = await axios.get(
        `${ORDER_URL()}/api/user/${payload.userId}`,
        { headers: INTERNAL_HEADERS() }
      );

      const toEmail = userRes.data?.email;
      if (toEmail) {
        await emailService.sendOrderFinalStateEmail(toEmail, payload.status, payload.reason || '');
      }

      channel.ack(msg);
    } catch (err) {
      console.error('Notification consumer order-finalized error:', err.message);
      // Do not block saga completion because of email issues.
      channel.ack(msg);
    }
  });

  console.log('NotificationService RabbitMQ consumers started');
  return { connection, channel };
}

module.exports = { startOrderFinalizedConsumer };
