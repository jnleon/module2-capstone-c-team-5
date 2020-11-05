/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP (1000) [transfer_id]
      ,[transfer_type_id]
      ,[transfer_status_id]
      ,[account_from]
      ,[account_to]
      ,[amount]
  FROM [tenmo].[dbo].[transfers]


SELECT t.transfer_id, t.transfer_type_id, t.transfer_status_id, t.account_from, t.account_to, t.amount, u.username
FROM transfers t
INNER JOIN accounts a ON t.account_to = a.account_id
INNER JOIN users u ON a.user_id = u.user_id
WHERE t.account_from = 1;

SELECT t.transfer_id, t.transfer_type_id, t.transfer_status_id, t.account_from, t.account_to, t.amount, u.username
FROM transfers t
INNER JOIN accounts a ON t.account_from = a.account_id
INNER JOIN users u ON a.user_id = u.user_id

SELECT t.transfer_id, t.transfer_type_id, t.transfer_status_id, t.account_from, t.account_to, t.amount, u.username AS "UserNameFrom", ub.username as "UserNameTo"
FROM transfers t
INNER JOIN accounts a ON t.account_from = a.account_id
INNER JOIN users u ON a.user_id = u.user_id
INNER JOIN accounts ab ON t.account_to = ab.account_id
INNER JOIN users ub ON ab.user_id = ub.user_id
WHERE u.user_id = 1 OR ub.user_id = 1;

SELECT *
FROM users
INNER JOIN 