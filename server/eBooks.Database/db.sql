-- ========================================
-- CREATE DATABASE
-- ========================================
CREATE DATABASE eBooks;
GO

USE eBooks;
GO

-- ========================================
-- TABLES
-- ========================================

CREATE TABLE Roles (
    RoleId INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100) NOT NULL
);

CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY,
    UserName NVARCHAR(100) UNIQUE NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    PasswordSalt NVARCHAR(255) NOT NULL,
    FilePath NVARCHAR(255) NOT NULL,
    ModifiedAt DATETIME NOT NULL DEFAULT GETDATE(),
    IsEmailVerified BIT NOT NULL DEFAULT 0,
    VerificationToken NVARCHAR(100),
    TokenExpiry DATETIME,
    DeletionReason NVARCHAR(255),
    StripeAccountId NVARCHAR(100),
    RoleId INT NOT NULL,
    PublisherVerifiedById INT,
    FOREIGN KEY (RoleId) REFERENCES Roles(RoleId),
    FOREIGN KEY (PublisherVerifiedById) REFERENCES Users(UserId)
);

CREATE TABLE Authors (
    AuthorId INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100) NOT NULL UNIQUE,
    ModifiedAt DATETIME NOT NULL DEFAULT GETDATE(),
    ModifiedById INT,
    FOREIGN KEY (ModifiedById) REFERENCES Users(UserId)
);

CREATE TABLE Genres (
    GenreId INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100) NOT NULL UNIQUE,
    ModifiedAt DATETIME NOT NULL DEFAULT GETDATE(),
    ModifiedById INT,
    FOREIGN KEY (ModifiedById) REFERENCES Users(UserId)
);

CREATE TABLE Languages (
    LanguageId INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100) NOT NULL UNIQUE,
    ModifiedAt DATETIME NOT NULL DEFAULT GETDATE(),
    ModifiedById INT,
    FOREIGN KEY (ModifiedById) REFERENCES Users(UserId)
);

CREATE TABLE Books (
    BookId INT PRIMARY KEY IDENTITY,
    Title NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX),
    FilePath NVARCHAR(100) NOT NULL,
    Price DECIMAL(10,2),
    NumberOfPages INT,
    NumberOfViews INT NOT NULL DEFAULT 0,
    LanguageId INT,
    PublisherId INT NOT NULL,
    ModifiedAt DATETIME NOT NULL DEFAULT GETDATE(),
    StateMachine NVARCHAR(50) NOT NULL DEFAULT 'draft',
    ReviewedById INT,
    DiscountPercentage INT,
    DiscountStart DATETIME,
    DiscountEnd DATETIME,
    RejectionReason NVARCHAR(255),
    DeletionReason NVARCHAR(255),
    FOREIGN KEY (LanguageId) REFERENCES Languages(LanguageId),
    FOREIGN KEY (PublisherId) REFERENCES Users(UserId),
    FOREIGN KEY (ReviewedById) REFERENCES Users(UserId)
);

CREATE TABLE BookAuthors (
    BookId INT,
    AuthorId INT,
    ModifiedAt DATETIME NOT NULL DEFAULT GETDATE(),
    PRIMARY KEY (BookId, AuthorId),
    FOREIGN KEY (BookId) REFERENCES Books(BookId),
    FOREIGN KEY (AuthorId) REFERENCES Authors(AuthorId)
);

CREATE TABLE BookGenres (
    BookId INT,
    GenreId INT,
    ModifiedAt DATETIME NOT NULL DEFAULT GETDATE(),
    PRIMARY KEY (BookId, GenreId),
    FOREIGN KEY (BookId) REFERENCES Books(BookId),
    FOREIGN KEY (GenreId) REFERENCES Genres(GenreId)
);

CREATE TABLE Wishlists (
    UserId INT,
    BookId INT,
    ModifiedAt DATETIME NOT NULL DEFAULT GETDATE(),
    PRIMARY KEY (UserId, BookId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (BookId) REFERENCES Books(BookId)
);

CREATE TABLE AccessRights (
    UserId INT NOT NULL,
    BookId INT NOT NULL,
    ModifiedAt DATETIME NOT NULL DEFAULT GETDATE(),
    IsFavorite BIT NOT NULL DEFAULT 0,
    IsHidden BIT NOT NULL DEFAULT 0,
    LastReadPage INT NOT NULL DEFAULT 0,
    PRIMARY KEY (UserId, BookId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (BookId) REFERENCES Books(BookId)
);

CREATE TABLE ReadingProgresses (
    UserId INT,
    BookId INT,
    ModifiedAt DATETIME NOT NULL DEFAULT GETDATE(),
    LastReadPage INT,
    PRIMARY KEY (UserId, BookId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (BookId) REFERENCES Books(BookId)
);

CREATE TABLE Reviews (
    UserId INT,
    BookId INT,
    ModifiedAt DATETIME NOT NULL DEFAULT GETDATE(),
    Rating INT NOT NULL,
    Comment NVARCHAR(MAX),
    ReportedById INT,
    ReportReason NVARCHAR(MAX),
    PRIMARY KEY (UserId, BookId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (BookId) REFERENCES Books(BookId),
    FOREIGN KEY (ReportedById) REFERENCES Users(UserId)
);

CREATE TABLE Reports (
    UserId INT NOT NULL,
    BookId INT NOT NULL,
    ModifiedAt DATETIME NOT NULL DEFAULT GETDATE(),
    Reason NVARCHAR(MAX),
    PRIMARY KEY (UserId, BookId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (BookId) REFERENCES Books(BookId)
);

CREATE TABLE Purchases (
    PurchaseId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    PublisherId INT NOT NULL,
    BookId INT NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    TotalPrice DECIMAL(18,2) NOT NULL,
    PaymentStatus NVARCHAR(50) NOT NULL,
    PaymentMethod NVARCHAR(50) NOT NULL,
    TransactionId NVARCHAR(255) NOT NULL,
    FailureMessage NVARCHAR(255),
    FailureCode NVARCHAR(50),
    FailureReason NVARCHAR(255),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (PublisherId) REFERENCES Users(UserId),
    FOREIGN KEY (BookId) REFERENCES Books(BookId)
);

CREATE TABLE PublisherFollows (
    UserId INT,
    PublisherId INT,
    ModifiedAt DATETIME NOT NULL DEFAULT GETDATE(),
    PRIMARY KEY (UserId, PublisherId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (PublisherId) REFERENCES Users(UserId)
);

CREATE TABLE Notifications (
    NotificationId INT PRIMARY KEY IDENTITY,
    BookId INT,
    PublisherId INT,
    UserId INT NOT NULL,
    Message NVARCHAR(MAX) NOT NULL,
    ModifiedAt DATETIME NOT NULL DEFAULT GETDATE(),
    IsRead BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (BookId) REFERENCES Books(BookId),
    FOREIGN KEY (PublisherId) REFERENCES Users(UserId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

CREATE TABLE Questions (
    QuestionId INT PRIMARY KEY IDENTITY,
    UserId INT NOT NULL,
    Question NVARCHAR(MAX) NOT NULL,
    Answer NVARCHAR(MAX),
    AnsweredById INT,
    ModifiedAt DATETIME NOT NULL DEFAULT GETDATE(),
    AnsweredAt DATETIME,
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (AnsweredById) REFERENCES Users(UserId)
);

-- ========================================
-- INSERT DATA
-- ========================================

-- Roles
SET IDENTITY_INSERT Roles ON;
INSERT INTO Roles (RoleId, Name) VALUES
(1, 'User'),
(2, 'Admin'),
(3, 'Moderator');
SET IDENTITY_INSERT Roles OFF;

-- Users
SET IDENTITY_INSERT Users ON;
INSERT INTO Users (UserId, UserName, FirstName, LastName, Email, PasswordHash, PasswordSalt, FilePath, ModifiedAt, IsEmailVerified, VerificationToken, TokenExpiry, DeletionReason, StripeAccountId, RoleId, PublisherVerifiedById) VALUES
(1, 'ikbalcaus', 'Ikbal', 'Čaušević', 'ikbalcaus@gmail.com', 'G5padN8V9FCeSZ6yrVouhvjucVFBjUAGn8g68yrHxVA=', 'xSqsH9S5lTU2k0MXRZABA9eRH7DEfuvJgn5TZHryUUE=', '868d4003145748148ed59986b437b069', '2025-10-05 16:42:33', 1, NULL, NULL, NULL, 'acct_1RRkBpD0ZyFV94kx', 2, 1),
(2, 'korisnik', 'Korisnik', 'Korisnik', 'korisnik@gmail.com', 'Ec59V8iH0Wf4OTSLdD6UOZTh7WwGjWJVuJZe2hLVA8E=', '4AzRN6LxhmdcwuMDSVWaiL/LgJ/SjmEFrhitVa5fLuc=', '2b836411e26146fda93987a6b570da3f', '2025-10-05 17:43:06', 1, NULL, NULL, NULL, 'acct_1SEu5UDDffKjsSk4', 1, 1),
(3, 'moderator', 'Moderator', 'Moderator', 'moderator@gmail.com', 'oa+iRyiF9jksNB+OcaQdu+QA1d/lCz+4kudPBmLbE6Q=', 'OIP6qrpkE2H66BjB5fv+rsMGqSHpcsxdlptVl3l4Ovc=', '8975385763e84182845dfa558098cc20', '2025-10-05 17:45:13', 0, NULL, NULL, NULL, 'acct_1SEu7XDTAAJUs0Kz', 3, NULL),
(4, 'admin', 'admin', 'admin', 'admin@gmail.com', 'cAkxSNbgYjCRV6UmFvVN4JwovOEqJWmjDiFnvgCzxvk=', 'wQo7VN7c/u45/dRVFovl5o4E9QUN6YY1afkWPJptCvY=', '0f7f6ffb9f6f4dc3b8a7cc7b25f31530', '2025-10-05 17:46:08', 0, NULL, NULL, NULL, 'acct_1SEu8QDWIdj4EDmp', 2, NULL),
(5, 'korisnik2', 'Korisnik', 'Korisnik', 'korisnik2@gmail.com', 'Ec59V8iH0Wf4OTSLdD6UOZTh7WwGjWJVuJZe2hLVA8E=', '4AzRN6LxhmdcwuMDSVWaiL/LgJ/SjmEFrhitVa5fLuc=', '2b836411e26146fda93987a6b570da3f', '2025-10-05 17:43:06', 0, NULL, NULL, NULL, 'acct_1SEu5UDDffKjsSk4', 1, NULL),
(6, 'korisnik3', 'Korisnik', 'Korisnik', 'korisnik3@gmail.com', 'Ec59V8iH0Wf4OTSLdD6UOZTh7WwGjWJVuJZe2hLVA8E=', '4AzRN6LxhmdcwuMDSVWaiL/LgJ/SjmEFrhitVa5fLuc=', '2b836411e26146fda93987a6b570da3f', '2025-10-05 17:43:06', 0, NULL, NULL, NULL, 'acct_1SEu5UDDffKjsSk4', 1, NULL),
(7, 'korisnik4', 'Korisnik', 'Korisnik', 'korisnik4@gmail.com', 'Ec59V8iH0Wf4OTSLdD6UOZTh7WwGjWJVuJZe2hLVA8E=', '4AzRN6LxhmdcwuMDSVWaiL/LgJ/SjmEFrhitVa5fLuc=', '2b836411e26146fda93987a6b570da3f', '2025-10-05 17:43:06', 0, NULL, NULL, NULL, 'acct_1SEu5UDDffKjsSk4', 1, NULL),
(8, 'korisnik5', 'Korisnik', 'Korisnik', 'korisnik5@gmail.com', 'Ec59V8iH0Wf4OTSLdD6UOZTh7WwGjWJVuJZe2hLVA8E=', '4AzRN6LxhmdcwuMDSVWaiL/LgJ/SjmEFrhitVa5fLuc=', '2b836411e26146fda93987a6b570da3f', '2025-10-05 17:43:06', 0, NULL, NULL, NULL, 'acct_1SEu5UDDffKjsSk4', 1, NULL);
SET IDENTITY_INSERT Users OFF;

-- ========================================
-- Authors
-- ========================================
SET IDENTITY_INSERT Authors ON;
INSERT INTO Authors (AuthorId, Name, ModifiedAt, ModifiedById) VALUES
(1, 'Antun Hangi', '2025-10-05 14:56:11', 1),
(2, 'Jan Willem Honig', '2025-10-05 15:03:15', 1),
(3, 'Dr. Se''id el - Kerjibani', '2025-10-05 15:09:00', 1),
(4, 'Hamza Omer Mutevelić', '2025-10-05 15:27:37', 1),
(5, 'Dr.Omer Nakičević', '2025-10-05 15:33:19', 1),
(6, 'Munir Ahmetspaihić', '2025-10-05 15:33:18', 1),
(7, 'Dr. Muhammed bin Abdullah bin Ajid el-Gabban', '2025-10-05 15:38:39', 1);
SET IDENTITY_INSERT Authors OFF;

-- ========================================
-- Genres
-- ========================================
SET IDENTITY_INSERT Genres ON;
INSERT INTO Genres (GenreId, Name, ModifiedAt, ModifiedById) VALUES
(1, 'Historija Bosne', '2025-10-05 14:56:11', 1),
(2, 'Islam', '2025-10-05 15:09:00', 1),
(3, 'Arapski jezik', '2025-10-05 15:27:36', 1);
SET IDENTITY_INSERT Genres OFF;

-- ========================================
-- Languages
-- ========================================
SET IDENTITY_INSERT Languages ON;
INSERT INTO Languages (LanguageId, Name, ModifiedAt, ModifiedById) VALUES
(1, 'English', '2025-10-05 16:40:07', NULL),
(2, 'Bosnian', '2025-10-05 14:56:10', 1),
(3, 'Spanish', '2025-10-05 18:53:37', NULL),
(4, 'German', '2025-10-05 18:53:53', NULL),
(5, 'French', '2025-10-05 18:54:06', NULL),
(6, 'Russian', '2025-10-05 18:54:15', NULL),
(7, 'Turkish', '2025-10-05 18:54:19', NULL),
(8, 'Arabic', '2025-10-05 18:54:45', NULL),
(9, 'Persian', '2025-10-05 18:54:48', NULL),
(10, 'Indian', '2025-10-05 18:54:51', NULL),
(11, 'Chinese', '2025-10-05 18:55:08', NULL),
(12, 'Japanese', '2025-10-05 18:55:14', NULL);
SET IDENTITY_INSERT Languages OFF;

-- ========================================
-- Books
-- ========================================
SET IDENTITY_INSERT Books ON;
INSERT INTO Books (BookId, Title, Description, FilePath, Price, NumberOfPages, NumberOfViews, LanguageId, PublisherId, ModifiedAt, StateMachine, ReviewedById, DiscountPercentage, DiscountStart, DiscountEnd) VALUES
(1, 'Život i običaji muslimana u Bosni i Hercegovini', 'Knjiga koja je zapis o iskustvu historijskog života u Bosni i Hercegovini.', '4b318aec9f684b4abd3a1f3768e281d4', 24.00, 224, 8, 2, 1, '2025-10-05 16:56:14', 'approve', 1, NULL, NULL, NULL),
(2, 'Srebrenica - Hronika ratnog zločina', 'Knjiga koja govori o stradanjima Srebreničana.', 'aec4a9f3be8a48b3abf1ab350fb10e1b', 21.00, 213, 4, 2, 2, '2025-10-05 17:03:19', 'approve', 1, NULL, NULL, NULL),
(3, 'Ostvareni san - Životna priča kurra hafiza Abdut-T-Tevaba Raudana', 'Neprestana su čudesa koja Kur’an pruža onima koji se stalno druže sa njim...', '8388225fe3cf42e985f030ced5f6a2b2', 0.00, 27, 5, 2, 1, '2025-10-05 17:09:04', 'approve', 1, NULL, NULL, NULL),
(4, 'Sufara - Arapsko pismo - Harfovi', 'Pred nama je Sufara koja u novoj metodologiji i pristupu obrađuje harfove...', '3c30d5fb29774c57bc80c60be74d008d', 14.00, 141, 1, 2, 1, '2025-10-05 17:27:42', 'approve', 1, NULL, NULL, NULL),
(5, 'Priručnik Arapskog jezika 1', 'Ovaj udžbenik je zamišljen kao prvi kurs arapskog jezika...', 'ed3d7b129de545efbde9aab0c27e8aa4', 22.00, 223, 4, 2, 1, '2025-10-05 17:33:22', 'approve', 1, NULL, NULL, NULL),
(6, 'Olakšano poimanje TEDŽVIDSKIH pravila za halke Kur''ana', 'Ovo djelo je prvi hvale vrijedan prijevod na našem jeziku...', '080c865fdd054bde830588b38432e56a', 23.00, 235, 9, 9, 1, '2025-10-05 17:38:42', 'approve', 1, 50, '2025-10-04', '2025-12-31');
SET IDENTITY_INSERT Books OFF;

-- ========================================
-- BookAuthors
-- ========================================
SET IDENTITY_INSERT BookAuthors ON;
INSERT INTO BookAuthors (BookId, AuthorId, ModifiedAt) VALUES
(1, 1, '2025-10-05 14:56:11'),
(2, 2, '2025-10-05 15:03:15'),
(3, 3, '2025-10-05 15:23:30'),
(4, 4, '2025-10-05 15:27:37'),
(5, 5, '2025-10-05 15:33:40'),
(5, 6, '2025-10-05 15:33:39'),
(6, 7, '2025-10-05 15:38:39');
SET IDENTITY_INSERT BookAuthors OFF;

-- ========================================
-- BookGenres
-- ========================================
SET IDENTITY_INSERT BookGenres ON;
INSERT INTO BookGenres (BookId, GenreId, ModifiedAt) VALUES
(1, 1, '2025-10-05 14:56:11'),
(2, 1, '2025-10-05 15:03:15'),
(3, 2, '2025-10-05 15:23:30'),
(4, 2, '2025-10-05 15:27:37'),
(4, 3, '2025-10-05 15:27:36'),
(5, 3, '2025-10-05 15:33:40'),
(6, 2, '2025-10-05 15:38:39');
SET IDENTITY_INSERT BookGenres OFF;

-- ========================================
-- AccessRights
-- ========================================
INSERT INTO AccessRights (UserId, BookId, ModifiedAt, IsFavorite, IsHidden, LastReadPage) VALUES
(2, 6, '2025-10-05 19:10:29', 0, 0, 0),
(1, 1, '2025-10-05 19:32:27', 0, 0, 0),
(1, 2, '2025-10-05 19:32:30', 0, 0, 0),
(3, 1, '2025-10-05 19:32:42', 0, 0, 0),
(3, 2, '2025-10-05 19:32:43', 0, 0, 0),
(4, 3, '2025-10-05 19:33:23', 0, 0, 0),
(4, 6, '2025-10-05 19:33:25', 0, 0, 0),
(5, 3, '2025-10-05 19:44:18', 0, 0, 0),
(5, 6, '2025-10-05 19:44:20', 0, 0, 0),
(6, 4, '2025-10-05 19:44:24', 0, 0, 0),
(6, 5, '2025-10-05 19:44:36', 0, 0, 0),
(7, 4, '2025-10-05 19:44:43', 0, 0, 0),
(7, 5, '2025-10-05 19:44:45', 0, 0, 0),
(8, 1, '2025-10-05 19:45:04', 0, 0, 0),
(8, 2, '2025-10-05 19:45:06', 0, 0, 0),
(8, 3, '2025-10-05 19:45:08', 0, 0, 0),
(8, 4, '2025-10-05 19:45:10', 0, 0, 0),
(8, 5, '2025-10-05 19:45:12', 0, 0, 0),
(8, 6, '2025-10-05 19:45:14', 0, 0, 0);

-- ========================================
-- Reviews
-- ========================================
INSERT INTO Reviews (UserId, BookId, ModifiedAt, Rating, Comment, ReportedById, ReportReason) VALUES
(2, 4, '2025-10-05 19:03:10', 1, 'Predobra knjiga', 1, 'Greškom stavio 1 zvjezdicu');

-- ========================================
-- Reports
-- ========================================
INSERT INTO Reports (UserId, BookId, ModifiedAt, Reason) VALUES
(2, 1, '2025-10-05 19:03:50', 'Prijava knjige');

-- ========================================
-- Purchases
-- ========================================
INSERT INTO Purchases (PurchaseId, UserId, PublisherId, BookId, CreatedAt, TotalPrice, PaymentStatus, PaymentMethod, TransactionId, FailureMessage, FailureCode, FailureReason) VALUES
(2, 2, 1, 4, '2025-10-05 19:00:18', 14.00, 'success', 'card', '1', NULL, NULL, NULL);

-- ========================================
-- Notifications
-- ========================================
INSERT INTO Notifications (NotificationId, BookId, PublisherId, UserId, Message, ModifiedAt, IsRead) VALUES
(1, NULL, 2, 2, 'Verification email sent to your email', '2025-10-05 19:11:35', 0),
(2, NULL, 2, 2, 'Sucessfully verified email', '2025-10-05 19:11:36', 1),
(3, 6, NULL, 2, 'Book Olakšano poimanje TEDŽVIDSKIH pravila za halke Kur''ana is on a discount', '2025-10-05 19:11:37', 1);

-- ========================================
-- Questions
-- ========================================
INSERT INTO Questions (QuestionId, UserId, Question, Answer, AnsweredById, ModifiedAt, AnsweredAt) VALUES
(1, 2, 'Da li postoji web aplikacija?', NULL, NULL, '2025-10-05 19:07:49', NULL),
(2, 2, 'Da li mogu besplatno čitati sve knjige?', 'Žao nam je, ali ne možete čitati besplato sve knjige', 1, '2025-10-05 19:08:24', '2025-10-05 19:08:24');
