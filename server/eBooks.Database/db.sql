-- dotnet ef dbcontext scaffold "Name=Database" Microsoft.EntityFrameworkCore.SqlServer --output-dir Models --context-dir . --force --project eBooks.Database/eBooks.Database.csproj --startup-project eBooks.API/eBooks.API.csproj

CREATE DATABASE eBooks;
GO

USE eBooks;
GO

CREATE TABLE Roles (
    RoleId INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100) NOT NULL
);

CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY,
    UserName NVARCHAR(100) UNIQUE,
    FirstName NVARCHAR(100),
    LastName NVARCHAR(100),
    Email NVARCHAR(100) UNIQUE,
    PasswordHash NVARCHAR(255),
    PasswordSalt NVARCHAR(255),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    isDeleted BIT DEFAULT 0,
    RoleId INT NOT NULL,
    StripeAccountId NVARCHAR(255) NULL,
    FOREIGN KEY (RoleId) REFERENCES Roles(RoleId)
);

CREATE TABLE Authors (
    AuthorId INT PRIMARY KEY IDENTITY,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL
);

CREATE TABLE Genres (
    GenreId INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE Languages (
    LanguageId INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100) NOT NULL UNIQUE,
    Abbreviation NVARCHAR(10) NOT NULL UNIQUE
);

CREATE TABLE Books (
    BookId INT PRIMARY KEY IDENTITY,
    Title NVARCHAR(255),
    PdfPath NVARCHAR(255),
    Price DECIMAL(10,2),
    NumberOfPages INT,
    LanguageId INT NOT NULL,
    PublisherId INT NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    StateMachine NVARCHAR(50) DEFAULT 'draft',
    RejectionReason NVARCHAR(500),
    isDeleted BIT DEFAULT 0,
    FOREIGN KEY (LanguageId) REFERENCES Languages(LanguageId),
    FOREIGN KEY (PublisherId) REFERENCES Users(UserId)
);

CREATE TABLE BookGenres (
    BookId INT,
    GenreId INT,
    ModifiedAt DATETIME NOT NULL DEFAULT GETDATE(),
    PRIMARY KEY (BookId, GenreId),
    FOREIGN KEY (BookId) REFERENCES Books(BookId),
    FOREIGN KEY (GenreId) REFERENCES Genres(GenreId)
);

CREATE TABLE BookAuthors (
    BookId INT,
    AuthorId INT,
    ModifiedAt DATETIME NOT NULL DEFAULT GETDATE(),
    PRIMARY KEY (BookId, AuthorId),
    FOREIGN KEY (BookId) REFERENCES Books(BookId),
    FOREIGN KEY (AuthorId) REFERENCES Authors(AuthorId)
);

CREATE TABLE BookImages (
    ImageId INT PRIMARY KEY IDENTITY,
    BookId INT,
    ImagePath NVARCHAR(255),
    ModifiedAt DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (BookId) REFERENCES Books(BookId)
);

CREATE TABLE Favorites (
    UserId INT,
    BookId INT,
    ModifiedAt DATETIME NOT NULL DEFAULT GETDATE(),
    PRIMARY KEY (UserId, BookId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (BookId) REFERENCES Books(BookId)
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
    Hidden BIT NOT NULL DEFAULT 0,
    PRIMARY KEY (UserId, BookId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (BookId) REFERENCES Books(BookId),
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

CREATE TABLE dbo.Purchases
(
    PurchaseId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    PublisherId INT NOT NULL,
    BookId INT NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    TotalPrice DECIMAL(18,2) NOT NULL,
    PaymentStatus VARCHAR(50) NOT NULL,
    PaymentMethod VARCHAR(50) NOT NULL,
    TransactionId VARCHAR(255) NOT NULL,
    FailureMessage VARCHAR(255),
    FailureCode VARCHAR(50),
    FailureReason VARCHAR(255),
    CONSTRAINT FK_Purchases_User FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_Purchases_Publisher FOREIGN KEY (PublisherId) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_Purchases_Book FOREIGN KEY (BookId) REFERENCES dbo.Books(BookId)
);

CREATE TABLE PublisherFollows (
    UserId INT,
    PublisherId INT,
    ModifiedAt DATETIME NOT NULL DEFAULT GETDATE(),
    PRIMARY KEY (UserId, PublisherId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (PublisherId) REFERENCES Users(UserId)
);

CREATE TABLE Reviews (
    UserId INT,
    BookId INT,
    ModifiedAt DATETIME NOT NULL DEFAULT GETDATE(),
    Rating INT,
    Comment NVARCHAR(MAX),
    PRIMARY KEY (UserId, BookId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (BookId) REFERENCES Books(BookId)
);

CREATE TABLE Notifications (
    NotificationId INT PRIMARY KEY IDENTITY,
    BookId INT NULL,
    PublisherId INT NULL,
    UserId INT NOT NULL,
    Message NVARCHAR(MAX),
    ModifiedAt DATETIME NOT NULL DEFAULT GETDATE(),
    IsRead BIT DEFAULT 0,
    FOREIGN KEY (BookId) REFERENCES Books(BookId),
    FOREIGN KEY (PublisherId) REFERENCES Users(UserId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

CREATE TABLE PublisherVerification (
    VerificationId INT PRIMARY KEY IDENTITY,
    PublisherId INT UNIQUE,
    AdminId INT NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (PublisherId) REFERENCES Users(UserId),
    FOREIGN KEY (AdminId) REFERENCES Users(UserId)
);
