-- dotnet ef dbcontext scaffold "Name=Database" Microsoft.EntityFrameworkCore.SqlServer --output-dir Models --context-dir . --force --project eBooks.Database/eBooks.Database.csproj --startup-project eBooks.API/eBooks.API.csproj

-- Kreiranje baze
CREATE DATABASE eBooks;
GO

USE eBooks;
GO

-- Tabela: Roles
CREATE TABLE Roles (
    RoleId INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100) NOT NULL
);

-- Tabela: Users
CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY,
    UserName NVARCHAR(100) UNIQUE,
    FirstName NVARCHAR(100),
    LastName NVARCHAR(100),
    Email NVARCHAR(100) UNIQUE,
    PasswordHash NVARCHAR(255),
    PasswordSalt NVARCHAR(255),
    RegistrationDate DATETIME DEFAULT GETDATE(),
    isDeleted BIT DEFAULT 0,
    RoleId INT NOT NULL,
    StripeAccountId NVARCHAR(255) NULL,
    FOREIGN KEY (RoleId) REFERENCES Roles(RoleId)
);

-- Tabela: Authors
CREATE TABLE Authors (
    AuthorId INT PRIMARY KEY IDENTITY,
    FirstName NVARCHAR(100),
    LastName NVARCHAR(100)
);

-- Tabela: Genres
CREATE TABLE Genres (
    GenreId INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100)
);

-- Tabela: Languages
CREATE TABLE Languages (
    LanguageId INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100) NOT NULL UNIQUE,
    Abbreviation NVARCHAR(10) NOT NULL UNIQUE
);

-- Tabela: Books
CREATE TABLE Books (
    BookId INT PRIMARY KEY IDENTITY,
    Title NVARCHAR(255),
    PdfPath NVARCHAR(255),
    Price DECIMAL(10,2),
    NumberOfPages INT,
    LanguageId INT NOT NULL,
    PublisherId INT NOT NULL,
    AddedDate DATETIME DEFAULT GETDATE(),
    StateMachine NVARCHAR(50) DEFAULT 'draft',
    RejectionReason NVARCHAR(500),
    isDeleted BIT DEFAULT 0,
    FOREIGN KEY (LanguageId) REFERENCES Languages(LanguageId),
    FOREIGN KEY (PublisherId) REFERENCES Users(UserId)
);

-- Tabela: BookGenres
CREATE TABLE BookGenres (
    BookId INT,
    GenreId INT,
    IsPrimary BIT DEFAULT 0,
    PRIMARY KEY (BookId, GenreId),
    FOREIGN KEY (BookId) REFERENCES Books(BookId),
    FOREIGN KEY (GenreId) REFERENCES Genres(GenreId)
);

-- Tabela: BookAuthors
CREATE TABLE BookAuthors (
    BookId INT,
    AuthorId INT,
    IsPrimary BIT DEFAULT 0,
    PRIMARY KEY (BookId, AuthorId),
    FOREIGN KEY (BookId) REFERENCES Books(BookId),
    FOREIGN KEY (AuthorId) REFERENCES Authors(AuthorId)
);

-- Tabela: BookImages
CREATE TABLE BookImages (
    ImageId INT PRIMARY KEY IDENTITY,
    BookId INT,
    ImagePath NVARCHAR(255),
    ModifiedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (BookId) REFERENCES Books(BookId)
);

-- Tabela: Favorites
CREATE TABLE Favorites (
    UserId INT,
    BookId INT,
    ModifiedAt DATETIME DEFAULT GETDATE(),
    PRIMARY KEY (UserId, BookId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (BookId) REFERENCES Books(BookId)
);

-- Tabela: Wishlists
CREATE TABLE Wishlists (
    UserId INT,
    BookId INT,
    ModifiedAt DATETIME DEFAULT GETDATE(),
    PRIMARY KEY (UserId, BookId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (BookId) REFERENCES Books(BookId)
);

-- Tabela: ReadingProgresses
CREATE TABLE ReadingProgresses (
    UserId INT,
    BookId INT,
    LastReadPage INT,
    LastReadDate DATETIME DEFAULT GETDATE(),
    PRIMARY KEY (UserId, BookId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (BookId) REFERENCES Books(BookId)
);

-- Tabela: BookFollows
CREATE TABLE BookFollows (
    UserId INT,
    BookId INT,
    FollowDate DATETIME DEFAULT GETDATE(),
    PRIMARY KEY (UserId, BookId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (BookId) REFERENCES Books(BookId)
);

-- Tabela: Purchases
CREATE TABLE dbo.Purchases
(
    PurchaseId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    PublisherId INT NOT NULL,
    BookId INT NOT NULL,
    PurchaseDate DATETIME NOT NULL DEFAULT GETDATE(),
    TotalPrice DECIMAL(18,2) NOT NULL,
    PaymentStatus VARCHAR(50) NOT NULL,
    PaymentMethod VARCHAR(50),
    TransactionId VARCHAR(255),
    FailureMessage VARCHAR(255),
    FailureCode VARCHAR(50),
    FailureReason VARCHAR(255),
    CONSTRAINT FK_Purchases_User FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_Purchases_Publisher FOREIGN KEY (PublisherId) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_Purchases_Book FOREIGN KEY (BookId) REFERENCES dbo.Books(BookId)
);

-- Tabela: AccessRights
CREATE TABLE AccessRights (
    UserId INT NOT NULL,
    BookId INT NOT NULL,
    ModifiedAt DATETIME NOT NULL DEFAULT GETDATE(),
    Hidden BIT NOT NULL DEFAULT 0,
    PRIMARY KEY (UserId, BookId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (BookId) REFERENCES Books(BookId),
);


-- Tabela: PublisherFollows
CREATE TABLE PublisherFollows (
    UserId INT,
    PublisherId INT,
    FollowDate DATETIME DEFAULT GETDATE(),
    PRIMARY KEY (UserId, PublisherId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (PublisherId) REFERENCES Users(UserId)
);

-- Tabela: Reviews
CREATE TABLE Reviews (
    BookId INT,
    UserId INT,
    Rating INT,
    Comment NVARCHAR(MAX),
    ReviewDate DATETIME DEFAULT GETDATE(),
    PRIMARY KEY (BookId, UserId),
    FOREIGN KEY (BookId) REFERENCES Books(BookId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

-- Tabela: Notifications
CREATE TABLE Notifications (
    NotificationId INT PRIMARY KEY IDENTITY,
    BookId INT NULL,
    PublisherId INT NULL,
    UserId INT NOT NULL,
    Message NVARCHAR(MAX),
    CreatedDate DATETIME DEFAULT GETDATE(),
    IsRead BIT DEFAULT 0,
    FOREIGN KEY (BookId) REFERENCES Books(BookId),
    FOREIGN KEY (PublisherId) REFERENCES Users(UserId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

-- Tabela: PublisherVerification
CREATE TABLE PublisherVerification (
    VerificationId INT PRIMARY KEY IDENTITY,
    PublisherId INT UNIQUE,
    AdminId INT NOT NULL,
    VerificationDate DATETIME,
    FOREIGN KEY (PublisherId) REFERENCES Users(UserId),
    FOREIGN KEY (AdminId) REFERENCES Users(UserId)
);
