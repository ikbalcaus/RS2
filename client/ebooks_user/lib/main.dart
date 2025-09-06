import "dart:async";
import "package:ebooks_user/providers/access_rights_provider.dart";
import "package:ebooks_user/providers/auth_provider.dart";
import "package:ebooks_user/providers/authors_provider.dart";
import "package:ebooks_user/providers/books_provider.dart";
import "package:ebooks_user/providers/genres_provider.dart";
import "package:ebooks_user/providers/languages_provider.dart";
import "package:ebooks_user/providers/notifications_provider.dart";
import "package:ebooks_user/providers/publisher_follows_provider.dart";
import "package:ebooks_user/providers/purchases_provider.dart";
import "package:ebooks_user/providers/questions_provider.dart";
import "package:ebooks_user/providers/reports_provider.dart";
import "package:ebooks_user/providers/reviews_provider.dart";
import "package:ebooks_user/providers/stripe_provider.dart";
import "package:ebooks_user/providers/theme_provider.dart";
import "package:ebooks_user/providers/users_provider.dart";
import "package:ebooks_user/providers/wishlist_provider.dart";
import "package:ebooks_user/screens/books_screen.dart";
import "package:ebooks_user/screens/reset_password_screen.dart";
import "package:ebooks_user/utils/globals.dart";
import "package:flutter/material.dart";
import "package:provider/provider.dart";
import "package:uni_links/uni_links.dart";
import "package:easy_localization/easy_localization.dart";

final GlobalKey<NavigatorState> navigatorKey = GlobalKey<NavigatorState>();

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  await EasyLocalization.ensureInitialized();

  runApp(
    EasyLocalization(
      supportedLocales: const [Locale("en"), Locale("bs")],
      path: "assets/translations",
      fallbackLocale: const Locale("en"),
      child: MultiProvider(
        providers: [
          ChangeNotifierProvider(create: (_) => AccessRightsProvider()),
          ChangeNotifierProvider(create: (_) => AuthProvider()),
          ChangeNotifierProvider(create: (_) => AuthorsProvider()),
          ChangeNotifierProvider(create: (_) => BooksProvider()),
          ChangeNotifierProvider(create: (_) => GenresProvider()),
          ChangeNotifierProvider(create: (_) => LanguagesProvider()),
          ChangeNotifierProvider(create: (_) => NotificationsProvider()),
          ChangeNotifierProvider(create: (_) => PublisherFollowsProvider()),
          ChangeNotifierProvider(create: (_) => PurchasesProvider()),
          ChangeNotifierProvider(create: (_) => QuestionsProvider()),
          ChangeNotifierProvider(create: (_) => ReportsProvider()),
          ChangeNotifierProvider(create: (_) => ReviewsProvider()),
          ChangeNotifierProvider(create: (_) => StripeProvider()),
          ChangeNotifierProvider(create: (_) => ThemeProvider()),
          ChangeNotifierProvider(create: (_) => UsersProvider()),
          ChangeNotifierProvider(create: (_) => WishlistProvider()),
        ],
        child: const MyApp(),
      ),
    ),
  );
}

class MyApp extends StatefulWidget {
  const MyApp({super.key});

  @override
  State<MyApp> createState() => _MyAppState();
}

class _MyAppState extends State<MyApp> {
  StreamSubscription? _sub;
  String? _resetPasswordToken;

  @override
  void initState() {
    super.initState();
    _initUniLinks();
  }

  @override
  void dispose() {
    _sub?.cancel();
    super.dispose();
  }

  Future _initUniLinks() async {
    try {
      final initialUri = await getInitialUri();
      if (initialUri != null) {
        _handleIncomingUri(initialUri);
      }
    } catch (ex) {}
    _sub = uriLinkStream.listen((Uri? uri) {
      if (uri != null) {
        _handleIncomingUri(uri);
      }
    }, onError: (err) {});
  }

  void _handleIncomingUri(Uri uri) {
    if (uri.scheme == "ebooks" && uri.host == "reset-password") {
      setState(() {
        _resetPasswordToken = uri.queryParameters["token"];
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      navigatorKey: navigatorKey,
      theme: ThemeData(
        appBarTheme: const AppBarTheme(
          backgroundColor: Globals.backgroundColor,
          foregroundColor: Colors.white,
          titleTextStyle: TextStyle(
            fontWeight: FontWeight.bold,
            fontSize: 20,
            color: Colors.white,
          ),
        ),
      ),
      darkTheme: ThemeData(
        brightness: Brightness.dark,
        appBarTheme: const AppBarTheme(
          backgroundColor: Globals.backgroundColor,
          foregroundColor: Colors.white,
          titleTextStyle: TextStyle(
            fontWeight: FontWeight.bold,
            fontSize: 20,
            color: Colors.white,
          ),
        ),
      ),
      themeMode: context.watch<ThemeProvider>().themeMode,
      localizationsDelegates: context.localizationDelegates,
      supportedLocales: context.supportedLocales,
      locale: context.locale,
      home: _resetPasswordToken != null
          ? ResetPasswordScreen(token: _resetPasswordToken!)
          : const BooksScreen(),
      debugShowCheckedModeBanner: false,
    );
  }
}

// flutter pub run build_runner build
