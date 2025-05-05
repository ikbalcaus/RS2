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
    UserName NVARCHAR(100),
    FirstName NVARCHAR(100),
    LastName NVARCHAR(100),
    Email NVARCHAR(100) UNIQUE,
    PasswordHash NVARCHAR(255),
    PasswordSalt NVARCHAR(255),
    RegistrationDate DATETIME DEFAULT GETDATE(),
    RoleId INT NOT NULL,
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

-- Tabela: Books
CREATE TABLE Books (
    BookId INT PRIMARY KEY IDENTITY,
    Title NVARCHAR(255),
    PdfPath NVARCHAR(255),
    Price DECIMAL(10,2),
    PublisherId INT NOT NULL,
    AddedDate DATETIME DEFAULT GETDATE(),
    StateMachine NVARCHAR(50) DEFAULT 'draft',
    RejectionReason NVARCHAR(500),
    isDeleted BIT DEFAULT 0,
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
    AddedDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (BookId) REFERENCES Books(BookId)
);

-- Tabela: Favorites
CREATE TABLE Favorites (
    UserId INT,
    BookId INT,
    AddedDate DATETIME DEFAULT GETDATE(),
    PRIMARY KEY (UserId, BookId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (BookId) REFERENCES Books(BookId)
);

-- Tabela: Wishlists
CREATE TABLE Wishlists (
    UserId INT,
    BookId INT,
    AddedDate DATETIME DEFAULT GETDATE(),
    PRIMARY KEY (UserId, BookId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (BookId) REFERENCES Books(BookId)
);

-- Tabela: Purchases
CREATE TABLE Purchases (
    UserId INT,
    BookId INT,
    PurchaseDate DATETIME DEFAULT GETDATE(),
    TotalPrice DECIMAL(10,2),
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

-- Tabela: AccessRights
CREATE TABLE AccessRights (
    AccessRightId INT PRIMARY KEY IDENTITY,
    UserId INT NOT NULL,
    BookId INT NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (BookId) REFERENCES Books(BookId)
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
