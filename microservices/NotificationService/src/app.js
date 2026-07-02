require('dotenv').config();
const express = require('express');
const { startOrderFinalizedConsumer } = require('./messaging/orderFinalizedConsumer');

const app = express();
app.use(express.json());

// ── Routes ────────────────────────────────────────────────────────────────────
app.use('/api/notification', require('./routes/notificationRoutes'));

// Health-check
app.get('/health', (_, res) => res.json({ status: 'ok', service: 'NotificationService' }));

const PORT = process.env.PORT || 8084;

startOrderFinalizedConsumer()
	.then(() => {
		app.listen(PORT, () => console.log(`NotificationService running on port ${PORT}`));
	})
	.catch(err => {
		console.error('NotificationService RabbitMQ startup error:', err.message);
		process.exit(1);
	});
