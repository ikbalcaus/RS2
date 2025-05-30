import "dart:io";
import "package:admin/providers/auth_provider.dart";
import "package:admin/providers/books_provider.dart";
import "package:admin/screens/books_screen.dart";
import "package:admin/screens/login_screen.dart";
import "package:flutter/material.dart";
import "package:provider/provider.dart";
import "package:window_manager/window_manager.dart";

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
        ChangeNotifierProvider(create: (_) => BooksProvider()),
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
          title: "EBooks Dashboard",
          home: authProvider.isLoggedIn ? const BooksScreen() : LoginScreen(),
        );
      },
    );
  }
}
