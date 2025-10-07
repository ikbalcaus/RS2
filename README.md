# eBooks

Seminar paper for the course Software Development II

## Setup Instructions

Password for `.zip` files is `fit`

After cloning the repository, do the following:

- **Extract:** `fit-build-2025-10-05-env`
- **Place the `.env` file into root folder**
- **Open root folder and run the command: `docker compose up --build`**

Before using the application, please read the notes below in this README:

- **Extract:** `fit-build-2025-10-05-desktop`
- **Run** `eBooks_desktop.exe` located in the "Release" folder
- **Enter desktop credentials** which can be found in this README

- Before running the mobile application, make sure that the app does not already exist on the Android emulator; if it does, uninstall it first
- **Extract:** `fit-build-2025-10-05-mobile`
- After extraction, drag the `.apk` file from the "flutter-apk" folder and wait for the installation to complete
- Once the app is installed, launch it and enter the mobile credentials which can be found in this README

## Notes

- When you release a new book, you must submit a publish request. To do this, click the three dots in the top-right corner of the book details screen, then select Publish Book. After that, the administration will review the book to ensure everything is correct and approve it.

- If you want to make changes to an already published book, you must first hide the book, make the necessary edits, and then submit it for publishing again. You will need to wait for the administration’s approval before the changes go live.

- The book owner can view the content of the book without purchasing it. Users who do not own the book must purchase it in order to access its content.

- When a user wants to purchase a book and clicks the button displaying the price, they will be redirected to a web page where they must fill in their Stripe account details. The required information can be found in this README file. Once the transaction is successful, the book will appear in the user’s library. After you buy a book, you must open app again and refresh a book details page.

- Roles are made in hierarchy. Admin also has roles of moderator and user options, moderator also has user options, and user has only user options.

- If you want to test review and report functionality, you have to press three dots icon on the top right corner of the book detail screen.

- Instead of paypal, I implemented stripe that enables payment with credit card and link. I could implement paypal because of their strong policy.

- **Please wait few seconds until app loads!**

## Credentials

### Desktop application

#### Administrator

- **Email:** `admin@gmail.com`
- **Password:** `Admin123!`

#### Moderator

- **Email:** `moderator@gmail.com`
- **Password:** `Moderator123!`

### Mobile application

#### User

- **Email:** `korisnik@gmail.com`
- **Password:** `Korisnik123!`

### Stripe

- **Card number:** `4242 4242 4242 4242`
- **Expiration date:** `12/34` (arbitrary)
- **CVC:** `123` (arbitrary)
- **ZIP Code:** `12345` (arbitrary)

## RabbitMQ

- **RabbitMQ** RabbitMQ is used for sending emails to users to notify them about various actions in the application.
- Emails are sent in the following cases: account reactivated, account deactivated, book reactivated, book deactivated, book on discount (sent to all users who follow that book), book reviewed by the administrator, email verified, password reset requested (works only on mobile devices), payment completed, update on a book if the user follows the publisher who released that book, publisher verified by the admin, and user question answered by the admin.
- **Notification are made only with email!**
