-- Run this schema against the database configured for the application.

-- Dumping structure for table endurancedatabase.notifier_settings
CREATE TABLE IF NOT EXISTS `notifier_settings` (
  `id` int NOT NULL AUTO_INCREMENT,
  `Type` enum('email','ntfy','discord') NOT NULL,
  `settings` json NOT NULL,
  `created_at` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=20 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Data exporting was unselected.

-- Dumping structure for table endurancedatabase.verified_emails
CREATE TABLE IF NOT EXISTS `verified_emails` (
  `id` int NOT NULL AUTO_INCREMENT,
  `email` varchar(320) NOT NULL,
  `is_verified` tinyint(1) NOT NULL DEFAULT 0,
  `verification_token` varchar(64) DEFAULT NULL,
  `verification_expires_at` datetime DEFAULT NULL,
  `last_verification_email_sent_at` datetime DEFAULT NULL,
  `unsubscribe_token` varchar(64) NOT NULL,
  `verified_at` datetime DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `email` (`email`),
  UNIQUE KEY `verification_token` (`verification_token`),
  UNIQUE KEY `unsubscribe_token` (`unsubscribe_token`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping structure for table endurancedatabase.watched_classes
CREATE TABLE IF NOT EXISTS `watched_classes` (
  `id` int NOT NULL AUTO_INCREMENT,
  `venue_id` varchar(50) NOT NULL DEFAULT '',
  `class_id` varchar(50) NOT NULL DEFAULT '',
  `is_active` tinyint(1) NOT NULL,
  `is_pending_verification` tinyint(1) NOT NULL DEFAULT 0,
  `created_at` datetime DEFAULT CURRENT_TIMESTAMP,
  `notifier_settings_id` int DEFAULT NULL,
  `verified_email_id` int DEFAULT NULL,
  `start_datetime` datetime NOT NULL,
  PRIMARY KEY (`id`),
  KEY `notifier_settings_id` (`notifier_settings_id`),
  KEY `verified_email_id` (`verified_email_id`),
  CONSTRAINT `watched_classes_ibfk_1` FOREIGN KEY (`notifier_settings_id`) REFERENCES `notifier_settings` (`id`),
  CONSTRAINT `watched_classes_ibfk_2` FOREIGN KEY (`verified_email_id`) REFERENCES `verified_emails` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=20 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
