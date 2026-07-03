require('dotenv').config();
const express = require('express');
const mongoose = require('mongoose');
const { startOrderEventsConsumer } = require('./messaging/orderEventsConsumer');
const { connectRedis } = require('./cache/redisClient');
const { logger, requestLogger } = require('./logger');

const app = express();
app.use(express.json());
app.use(requestLogger);

app.use((_, res, next) => {
  res.setHeader('X-Container-Id', process.env.HOSTNAME || 'unknown');
  next();
});

// ── Routes ────────────────────────────────────────────────────────────────────
app.use('/api/gift', require('./routes/giftRoutes'));
app.use('/api/donor', require('./routes/donorRoutes'));

// Health-check
app.get('/health', (_, res) => res.json({ status: 'ok', service: 'ProductCatalogService' }));

// ── MongoDB connection ─────────────────────────────────────────────────────────
mongoose.connect(process.env.MONGO_URI)
  .then(async () => {
    logger.info('Connected to MongoDB', { database: 'catalogdb' });
    await connectRedis();
    await startOrderEventsConsumer();
    const PORT = process.env.PORT || 8081;
    app.listen(PORT, () => logger.info('ProductCatalogService started', { port: PORT }));
  })
  .catch(err => {
    logger.error('MongoDB connection error', { error: err });
    process.exit(1);
  });
