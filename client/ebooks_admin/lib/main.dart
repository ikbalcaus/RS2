import "dart:io";
import "package:ebooks_admin/providers/auth_provider.dart";
import "package:ebooks_admin/providers/authors_provider.dart";
import "package:ebooks_admin/providers/books_provider.dart";
import "package:ebooks_admin/providers/genres_provider.dart";
import "package:ebooks_admin/providers/languages_provider.dart";
import "package:ebooks_admin/providers/purchases_provider.dart";
import "package:ebooks_admin/providers/questions_provider.dart";
import "package:ebooks_admin/providers/roles_provider.dart";
import "package:ebooks_admin/providers/users_provider.dart";
import "package:ebooks_admin/screens/books_screen.dart";
import "package:ebooks_admin/screens/login_screen.dart";
import "package:ebooks_admin/utils/globals.dart";
import "package:flutter/material.dart";
import "package:provider/provider.dart";
import "package:window_manager/window_manager.dart";

final GlobalKey<NavigatorState> navigatorKey = GlobalKey<NavigatorState>();

void main() async {
  WidgetsFlutterBinding.ensureInitialized();

  await windowManager.ensureInitialized();

  if (Platform.isWindows) {
    WindowManager.instance.setMinimumSize(const Size(800, 600));
  }

  runApp(
    MultiProvider(
      providers: [
        ChangeNotifierProvider(create: (_) => AuthProvider()),
        ChangeNotifierProvider(create: (_) => AuthorsProvider()),
        ChangeNotifierProvider(create: (_) => BooksProvider()),
        ChangeNotifierProvider(create: (_) => GenresProvider()),
        ChangeNotifierProvider(create: (_) => LanguagesProvider()),
        ChangeNotifierProvider(create: (_) => PurchasesProvider()),
        ChangeNotifierProvider(create: (_) => QuestionsProvider()),
        ChangeNotifierProvider(create: (_) => RolesProvider()),
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
    return Consumer<AuthProvider>(
      builder: (context, authProvider, child) {
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
          home: authProvider.isLoggedIn
              ? const BooksScreen()
              : const LoginScreen(),
          debugShowCheckedModeBanner: false,
        );
      },
    );
  }
}

// flutter pub run build_runner build
