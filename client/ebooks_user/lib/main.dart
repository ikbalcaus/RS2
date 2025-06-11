import "package:ebooks_user/providers/auth_provider.dart";
import "package:ebooks_user/providers/authors_provider.dart";
import "package:ebooks_user/providers/books_provider.dart";
import "package:ebooks_user/providers/genres_provider.dart";
import "package:ebooks_user/providers/languages_provider.dart";
import "package:ebooks_user/providers/notifications_provider.dart";
import "package:ebooks_user/providers/purchases_provider.dart";
import "package:ebooks_user/providers/questions_provider.dart";
import "package:ebooks_user/providers/users_provider.dart";
import "package:ebooks_user/screens/books_screen.dart";
import "package:ebooks_user/utils/globals.dart";
import "package:flutter/material.dart";
import "package:provider/provider.dart";

final GlobalKey<NavigatorState> navigatorKey = GlobalKey<NavigatorState>();

void main() async {
  runApp(
    MultiProvider(
      providers: [
        ChangeNotifierProvider(create: (_) => AuthProvider()),
        ChangeNotifierProvider(create: (_) => AuthorsProvider()),
        ChangeNotifierProvider(create: (_) => BooksProvider()),
        ChangeNotifierProvider(create: (_) => GenresProvider()),
        ChangeNotifierProvider(create: (_) => LanguagesProvider()),
        ChangeNotifierProvider(create: (_) => NotificationsProvider()),
        ChangeNotifierProvider(create: (_) => PurchasesProvider()),
        ChangeNotifierProvider(create: (_) => QuestionsProvider()),
        ChangeNotifierProvider(create: (_) => UsersProvider()),
      ],
      child: MyApp(),
    ),
  );
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      navigatorKey: navigatorKey,
      theme: ThemeData(
        appBarTheme: const AppBarTheme(
          backgroundColor: Globals.backgroundColor,
          foregroundColor: Globals.color,
          titleTextStyle: TextStyle(
            fontWeight: FontWeight.bold,
            fontSize: 20,
            color: Globals.color,
          ),
        ),
      ),
      home: const BooksScreen(),
    );
  }
}

// flutter pub run build_runner build
