using eBooks.Database;
using Microsoft.EntityFrameworkCore;

namespace eBooks.API
{
    public static class DbSeeder
    {
        public static void SeedData(EBooksContext db)
        {
            try
            {
                var sql = @"
            -- Disable all constraints
            EXEC sp_msforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL';

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

            -- Authors
            SET IDENTITY_INSERT Authors ON;
            INSERT INTO Authors (AuthorId, Name, ModifiedAt, ModifiedById) VALUES
            (1, 'Antun Hangi', '2025-10-05 14:56:11', 1),
            (2, 'Jan Willem Honig', '2025-10-05 15:03:15', 1),
            (3, 'Dr. Seid el - Kerjibani', '2025-10-05 15:09:00', 1),
            (4, 'Hamza Omer Mutevelić', '2025-10-05 15:27:37', 1),
            (5, 'Dr.Omer Nakičević', '2025-10-05 15:33:19', 1),
            (6, 'Munir Ahmetspaihić', '2025-10-05 15:33:18', 1),
            (7, 'Dr. Muhammed bin Abdullah bin Ajid el-Gabban', '2025-10-05 15:38:39', 1);
            SET IDENTITY_INSERT Authors OFF;

            -- Genres
            SET IDENTITY_INSERT Genres ON;
            INSERT INTO Genres (GenreId, Name, ModifiedAt, ModifiedById) VALUES
            (1, 'Historija Bosne', '2025-10-05 14:56:11', 1),
            (2, 'Islam', '2025-10-05 15:09:00', 1),
            (3, 'Arapski jezik', '2025-10-05 15:27:36', 1);
            SET IDENTITY_INSERT Genres OFF;

            -- Languages
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

            -- Books
            SET IDENTITY_INSERT Books ON;
            INSERT INTO Books (BookId, Title, Description, FilePath, Price, NumberOfPages, NumberOfViews, LanguageId, PublisherId, ModifiedAt, StateMachine, ReviewedById, DiscountPercentage, DiscountStart, DiscountEnd) VALUES
            (1, 'Život i običaji muslimana u Bosni i Hercegovini', 'Knjiga koja je zapis o iskustvu historijskog života u Bosni i Hercegovini.', '4b318aec9f684b4abd3a1f3768e281d4', 24.00, 224, 8, 2, 1, '2025-10-05 16:56:14', 'approve', 1, NULL, NULL, NULL),
            (2, 'Srebrenica - Hronika ratnog zločina', 'Knjiga koja govori o stradanjima Srebreničana.', 'aec4a9f3be8a48b3abf1ab350fb10e1b', 21.00, 213, 4, 2, 2, '2025-10-05 17:03:19', 'approve', 1, NULL, NULL, NULL),
            (3, 'Ostvareni san - Životna priča kurra hafiza Abdut-T-Tevaba Raudana', 'Neprestana su čudesa koja Kur’an pruža onima koji se stalno druže sa njim...', '8388225fe3cf42e985f030ced5f6a2b2', 0.00, 27, 5, 2, 1, '2025-10-05 17:09:04', 'approve', 1, NULL, NULL, NULL),
            (4, 'Sufara - Arapsko pismo - Harfovi', 'Pred nama je Sufara koja u novoj metodologiji i pristupu obrađuje harfove...', '3c30d5fb29774c57bc80c60be74d008d', 14.00, 141, 1, 2, 1, '2025-10-05 17:27:42', 'approve', 1, NULL, NULL, NULL),
            (5, 'Priručnik Arapskog jezika 1', 'Ovaj udžbenik je zamišljen kao prvi kurs arapskog jezika...', 'ed3d7b129de545efbde9aab0c27e8aa4', 22.00, 223, 4, 2, 1, '2025-10-05 17:33:22', 'approve', 1, NULL, NULL, NULL),
            (6, 'Olakšano poimanje TEDŽVIDSKIH pravila za halke Kurana', 'Ovo djelo je prvi hvale vrijedan prijevod na našem jeziku...', '080c865fdd054bde830588b38432e56a', 23.00, 235, 9, 9, 1, '2025-10-05 17:38:42', 'approve', 1, 50, '2025-10-04', '2025-12-31');
            SET IDENTITY_INSERT Books OFF;

            -- BookAuthors
            INSERT INTO BookAuthors (BookId, AuthorId, ModifiedAt) VALUES
            (1, 1, '2025-10-05 14:56:11'),
            (2, 2, '2025-10-05 15:03:15'),
            (3, 3, '2025-10-05 15:23:30'),
            (4, 4, '2025-10-05 15:27:37'),
            (5, 5, '2025-10-05 15:33:40'),
            (5, 6, '2025-10-05 15:33:39'),
            (6, 7, '2025-10-05 15:38:39');

            -- BookGenres
            INSERT INTO BookGenres (BookId, GenreId, ModifiedAt) VALUES
            (1, 1, '2025-10-05 14:56:11'),
            (2, 1, '2025-10-05 15:03:15'),
            (3, 2, '2025-10-05 15:23:30'),
            (4, 2, '2025-10-05 15:27:37'),
            (4, 3, '2025-10-05 15:27:36'),
            (5, 3, '2025-10-05 15:33:40'),
            (6, 2, '2025-10-05 15:38:39');

            -- AccessRights
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

            -- Reviews
            INSERT INTO Reviews (UserId, BookId, ModifiedAt, Rating, Comment, ReportedById, ReportReason) VALUES
            (2, 4, '2025-10-05 19:03:10', 1, 'Predobra knjiga', 1, 'Greškom stavio 1 zvjezdicu');

            -- Reports
            INSERT INTO Reports (UserId, BookId, ModifiedAt, Reason) VALUES
            (2, 1, '2025-10-05 19:03:50', 'Prijava knjige');

            -- Purchases
            SET IDENTITY_INSERT Purchases ON;
            INSERT INTO Purchases (PurchaseId, UserId, PublisherId, BookId, CreatedAt, TotalPrice, PaymentStatus, PaymentMethod, TransactionId, FailureMessage, FailureCode, FailureReason) VALUES
            (1, 2, 1, 6, '2025-10-05 19:00:18', 23.00, 'success', 'card', '1', NULL, NULL, NULL),
            (2, 6, 2, 4, '2025-10-05 19:00:18', 14.00, 'success', 'card', '2', NULL, NULL, NULL),
            (3, 7, 2, 4, '2025-10-05 19:00:18', 14.00, 'success', 'card', '3', NULL, NULL, NULL),
            (4, 8, 2, 4, '2025-10-05 19:00:18', 14.00, 'success', 'card', '4', NULL, NULL, NULL);
            SET IDENTITY_INSERT Purchases OFF;

            -- Notifications
            SET IDENTITY_INSERT Notifications ON;
            INSERT INTO Notifications (NotificationId, BookId, PublisherId, UserId, Message, ModifiedAt, IsRead) VALUES
            (1, NULL, 2, 2, 'Verification email sent to your email', '2025-10-05 19:11:35', 0),
            (2, NULL, 2, 2, 'Sucessfully verified email', '2025-10-05 19:11:36', 1),
            (3, 6, NULL, 2, 'Book Olakšano poimanje TEDŽVIDSKIH pravila za halke Kurana is on a discount', '2025-10-05 19:11:37', 1);
            SET IDENTITY_INSERT Notifications OFF;

            -- Questions
            SET IDENTITY_INSERT Questions ON;
            INSERT INTO Questions (QuestionId, UserId, Question, Answer, AnsweredById, ModifiedAt, AnsweredAt) VALUES
            (1, 2, 'Da li postoji web aplikacija?', NULL, NULL, '2025-10-05 19:07:49', NULL),
            (2, 2, 'Da li mogu besplatno čitati sve knjige?', 'Žao nam je, ali ne možete čitati besplato sve knjige', 1, '2025-10-05 19:08:24', '2025-10-05 19:08:24');
            SET IDENTITY_INSERT Questions OFF;
            ";

                db.Database.ExecuteSqlRaw(sql);
            }
            catch (Exception ex)
            {
            }
        }
    }
}
