CREATE TABLE `verified_emails` (
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

ALTER TABLE `watched_classes`
  ADD COLUMN `is_pending_verification` tinyint(1) NOT NULL DEFAULT 0 AFTER `is_active`,
  ADD COLUMN `verified_email_id` int DEFAULT NULL AFTER `notifier_settings_id`;

-- Existing email recipients are trusted and migrated as already verified.
INSERT INTO `verified_emails` (`email`, `is_verified`, `unsubscribe_token`, `verified_at`)
SELECT
  emails.`email`,
  1,
  LOWER(HEX(RANDOM_BYTES(32))),
  CURRENT_TIMESTAMP
FROM (
  SELECT DISTINCT LOWER(TRIM(JSON_UNQUOTE(JSON_EXTRACT(ns.`settings`, '$.Email')))) AS `email`
  FROM `notifier_settings` ns
  WHERE ns.`Type` = 'email'
    AND JSON_UNQUOTE(JSON_EXTRACT(ns.`settings`, '$.Email')) IS NOT NULL
) emails;

UPDATE `watched_classes` wc
INNER JOIN `notifier_settings` ns ON ns.`id` = wc.`notifier_settings_id`
INNER JOIN `verified_emails` ve
  ON ve.`email` = LOWER(TRIM(JSON_UNQUOTE(JSON_EXTRACT(ns.`settings`, '$.Email'))))
SET wc.`verified_email_id` = ve.`id`
WHERE ns.`Type` = 'email';

ALTER TABLE `watched_classes`
  ADD KEY `verified_email_id` (`verified_email_id`),
  ADD CONSTRAINT `watched_classes_ibfk_2`
    FOREIGN KEY (`verified_email_id`) REFERENCES `verified_emails` (`id`);
