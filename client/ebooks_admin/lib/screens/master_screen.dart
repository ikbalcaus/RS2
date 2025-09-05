import "package:ebooks_admin/screens/authors_screen.dart";
import "package:ebooks_admin/screens/books_screen.dart";
import "package:ebooks_admin/screens/genres_screen.dart";
import "package:ebooks_admin/screens/languages_screen.dart";
import "package:ebooks_admin/screens/login_screen.dart";
import "package:ebooks_admin/screens/overview_screen.dart";
import "package:ebooks_admin/screens/purchases_screen.dart";
import "package:ebooks_admin/screens/questions_screen.dart";
import "package:ebooks_admin/screens/reports_screen.dart";
import "package:ebooks_admin/screens/reviews_screen.dart";
import "package:ebooks_admin/screens/users_screen.dart";
import "package:ebooks_admin/utils/globals.dart";
import "package:flutter/material.dart";
import "package:provider/provider.dart";
import "package:ebooks_admin/providers/auth_provider.dart";

class MasterScreen extends StatelessWidget {
  final Widget child;
  final bool showBackButton;

  const MasterScreen({
    super.key,
    this.showBackButton = false,
    required this.child,
  });

  @override
  Widget build(BuildContext context) {
    final authProvider = Provider.of<AuthProvider>(context);
    final isLoggedIn = authProvider.isLoggedIn;

    return Scaffold(
      appBar: AppBar(
        automaticallyImplyLeading: false,
        leading: showBackButton
            ? BackButton(onPressed: () => Navigator.pop(context))
            : (isLoggedIn
                  ? Builder(
                      builder: (context) => IconButton(
                        icon: const Icon(Icons.menu),
                        onPressed: () => Scaffold.of(context).openDrawer(),
                      ),
                    )
                  : null),
        title: const Text("E-Books Dashboard"),
        actions: isLoggedIn
            ? [
                Padding(
                  padding: const EdgeInsets.only(right: 10),
                  child: IconButton(
                    icon: const Icon(Icons.logout, color: Globals.color),
                    onPressed: () async {
                      await _showLogoutDialog(context, authProvider);
                    },
                  ),
                ),
              ]
            : null,
      ),
      drawer: showBackButton
          ? null
          : (isLoggedIn ? _buildDrawer(context, authProvider) : null),
      body: child,
    );
  }

  Widget _buildDrawer(BuildContext context, AuthProvider authProvider) {
    return Drawer(
      child: Column(
        children: [
          const DrawerHeader(
            decoration: BoxDecoration(color: Globals.backgroundColor),
            child: Align(
              alignment: Alignment.bottomLeft,
              child: Text(
                "Menu",
                style: TextStyle(color: Globals.color, fontSize: 24),
              ),
            ),
          ),
          ListTile(
            leading: const Icon(Icons.dashboard),
            title: const Text("Overview"),
            onTap: () {
              Navigator.pop(context);
              Future.delayed(Duration(milliseconds: 250), () {
                Navigator.pushReplacement(
                  context,
                  MaterialPageRoute(builder: (context) => OverviewScreen()),
                );
              });
            },
          ),
          ListTile(
            leading: const Icon(Icons.book),
            title: const Text("Books"),
            onTap: () {
              Navigator.pop(context);
              Future.delayed(Duration(milliseconds: 250), () {
                Navigator.pushReplacement(
                  context,
                  MaterialPageRoute(builder: (context) => BooksScreen()),
                );
              });
            },
          ),
          ListTile(
            leading: const Icon(Icons.person),
            title: const Text("Users"),
            onTap: () {
              Navigator.pop(context);
              Future.delayed(Duration(milliseconds: 250), () {
                Navigator.pushReplacement(
                  context,
                  MaterialPageRoute(builder: (context) => UsersScreen()),
                );
              });
            },
          ),
          ListTile(
            leading: const Icon(Icons.person_outline),
            title: const Text("Authors"),
            onTap: () {
              Navigator.pop(context);
              Future.delayed(Duration(milliseconds: 250), () {
                Navigator.pushReplacement(
                  context,
                  MaterialPageRoute(builder: (context) => AuthorsScreen()),
                );
              });
            },
          ),
          ListTile(
            leading: const Icon(Icons.category),
            title: const Text("Genres"),
            onTap: () {
              Navigator.pop(context);
              Future.delayed(Duration(milliseconds: 250), () {
                Navigator.pushReplacement(
                  context,
                  MaterialPageRoute(builder: (context) => GenresScreen()),
                );
              });
            },
          ),
          ListTile(
            leading: const Icon(Icons.language),
            title: const Text("Languages"),
            onTap: () {
              Navigator.pop(context);
              Future.delayed(Duration(milliseconds: 250), () {
                Navigator.pushReplacement(
                  context,
                  MaterialPageRoute(builder: (context) => LanguagesScreen()),
                );
              });
            },
          ),
          ListTile(
            leading: const Icon(Icons.question_mark),
            title: const Text("Questions"),
            onTap: () {
              Navigator.pop(context);
              Future.delayed(Duration(milliseconds: 250), () {
                Navigator.pushReplacement(
                  context,
                  MaterialPageRoute(builder: (context) => QuestionsScreen()),
                );
              });
            },
          ),
          ListTile(
            leading: const Icon(Icons.report),
            title: const Text("Reports"),
            onTap: () {
              Navigator.pop(context);
              Future.delayed(Duration(milliseconds: 250), () {
                Navigator.pushReplacement(
                  context,
                  MaterialPageRoute(builder: (context) => ReportsScreen()),
                );
              });
            },
          ),
          ListTile(
            leading: const Icon(Icons.reviews),
            title: const Text("Reviews"),
            onTap: () {
              Navigator.pop(context);
              Future.delayed(Duration(milliseconds: 250), () {
                Navigator.pushReplacement(
                  context,
                  MaterialPageRoute(builder: (context) => ReviewsScreen()),
                );
              });
            },
          ),
          ListTile(
            leading: const Icon(Icons.payment),
            title: const Text("Purchases"),
            onTap: () {
              Navigator.pop(context);
              Future.delayed(Duration(milliseconds: 250), () {
                Navigator.pushReplacement(
                  context,
                  MaterialPageRoute(builder: (context) => PurchasesScreen()),
                );
              });
            },
          ),
          const Spacer(),
          const Divider(),
          ListTile(
            leading: const Icon(Icons.logout),
            title: const Text("Logout"),
            onTap: () async {
              Navigator.pop(context);
              await _showLogoutDialog(context, authProvider);
            },
          ),
        ],
      ),
    );
  }

  Future _showLogoutDialog(
    BuildContext context,
    AuthProvider authProvider,
  ) async {
    await showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text("Logout"),
        content: const Text("Are you sure you want to logout?"),
        actions: [
          TextButton(
            onPressed: () {
              authProvider.logout();
              Navigator.pushAndRemoveUntil(
                context,
                MaterialPageRoute(builder: (_) => const LoginScreen()),
                (_) => false,
              );
            },
            child: const Text("Logout"),
          ),
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text("Cancel"),
          ),
        ],
      ),
    );
  }
}
