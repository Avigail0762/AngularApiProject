require('dotenv').config();
const express = require('express');
const mongoose = require('mongoose');
const { startOrderEventsConsumer } = require('./messaging/orderEventsConsumer');

const app = express();
app.use(express.json());

// ── Routes ────────────────────────────────────────────────────────────────────
app.use('/api/inventory', require('./routes/inventoryRoutes'));
app.use('/api/lottery',   require('./routes/lotteryRoutes'));
app.use('/api/purchases', require('./routes/purchasesRoutes'));

// Health-check
app.get('/health', (_, res) => res.json({ status: 'ok', service: 'InventoryService' }));

// ── MongoDB connection ─────────────────────────────────────────────────────────
mongoose.connect(process.env.MONGO_URI)
  .then(async () => {
    console.log('Connected to MongoDB (inventorydb)');
    await startOrderEventsConsumer();
    const PORT = process.env.PORT || 8083;
    app.listen(PORT, () => console.log(`InventoryService running on port ${PORT}`));
  })
  .catch(err => {
    console.error('MongoDB connection error:', err);
    process.exit(1);
  });
