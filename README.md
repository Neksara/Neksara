Instruction :
Before entering the application, please query the database.
copy this to query to table Neksara.

INSERT INTO Users
(Username, Email, Password, Role, CreatedAt)
VALUES
('admin', 'admin@gmail.com','admin123', 'Admin', GETDATE());
