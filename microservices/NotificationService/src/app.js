require('dotenv').config();
const express = require('express');
const { startOrderFinalizedConsumer } = require('./messaging/orderFinalizedConsumer');
const { logger, requestLogger } = require('./logger');

const app = express();
app.use(express.json());
app.use(requestLogger);

// ── Routes ────────────────────────────────────────────────────────────────────
app.use('/api/notification', require('./routes/notificationRoutes'));

// Health-check
app.get('/health', (_, res) => res.json({ status: 'ok', service: 'NotificationService' }));

const PORT = process.env.PORT || 8084;

startOrderFinalizedConsumer()
	.then(() => {
		app.listen(PORT, () => logger.info('NotificationService started', { port: PORT }));
	})
	.catch(err => {
		logger.error('NotificationService RabbitMQ startup error', { error: err });
		process.exit(1);
	});
