const winston = require('winston');
const { SeqTransport } = require('@datalust/winston-seq');

const transports = [
  new winston.transports.Console({
    format: winston.format.json()
  })
];

if (process.env.SEQ_URL) {
  transports.push(new SeqTransport({
    serverUrl: process.env.SEQ_URL,
    onError: error => console.error('Seq transport error', error)
  }));
}

const logger = winston.createLogger({
  level: process.env.LOG_LEVEL || 'info',
  format: winston.format.combine(
    winston.format.errors({ stack: true }),
    winston.format.timestamp(),
    winston.format.json()
  ),
  defaultMeta: { service: 'InventoryService' },
  transports
});

function requestLogger(req, res, next) {
  const startedAt = Date.now();
  res.on('finish', () => {
    logger.info('HTTP request completed', {
      method: req.method,
      path: req.originalUrl,
      statusCode: res.statusCode,
      durationMs: Date.now() - startedAt
    });
  });
  next();
}

function withCorrelationId(correlationId) {
  return correlationId ? logger.child({ CorrelationId: correlationId }) : logger;
}

module.exports = { logger, requestLogger, withCorrelationId };