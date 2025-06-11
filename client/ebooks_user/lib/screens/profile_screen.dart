import "package:ebooks_user/models/books/book.dart";
import "package:ebooks_user/models/search_result.dart";
import "package:ebooks_user/models/users/user.dart";
import "package:ebooks_user/providers/auth_provider.dart";
import "package:ebooks_user/providers/books_provider.dart";
import "package:ebooks_user/providers/users_provider.dart";
import "package:ebooks_user/screens/login_screen.dart";
import "package:ebooks_user/screens/master_screen.dart";
import "package:ebooks_user/utils/globals.dart";
import "package:ebooks_user/utils/helpers.dart";
import "package:flutter/material.dart";
import "package:provider/provider.dart";

class ProfileScreen extends StatefulWidget {
  const ProfileScreen({super.key});

  @override
  State<ProfileScreen> createState() => _ProfileScreenState();
}

class _ProfileScreenState extends State<ProfileScreen> {
  late UsersProvider _usersProvider;
  late BooksProvider _booksProvider;
  User? _user;
  SearchResult<Book>? _books;
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    if (AuthProvider.isLoggedIn) {
      _usersProvider = context.read<UsersProvider>();
      _booksProvider = context.read<BooksProvider>();
      _fetchUser();
      _fetchBooks();
    }
  }

  Future _fetchUser() async {
    setState(() => _isLoading = true);
    try {
      final user = await _usersProvider.getById(AuthProvider.userId);
      if (!mounted) return;
      setState(() => _user = user);
    } catch (ex) {
      if (!mounted) return;
      Helpers.showErrorMessage(context, ex);
    } finally {
      if (!mounted) return;
      setState(() => _isLoading = false);
    }
  }

  Future _fetchBooks() async {
    setState(() => _isLoading = true);
    try {
      final books = await _booksProvider.getPaged(
        filter: {"PublisherId": AuthProvider.userId},
      );
      if (!mounted) return;
      setState(() => _books = books);
    } catch (ex) {
      if (!mounted) return;

      Helpers.showErrorMessage(context, ex);
    } finally {
      if (!mounted) return;

      setState(() => _isLoading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    if (!AuthProvider.isLoggedIn) {
      return const MasterScreen(child: LoginScreen());
    }
    if (_isLoading) {
      return MasterScreen(
        child: const Center(child: CircularProgressIndicator()),
      );
    }
    return MasterScreen(child: _buildResultView());
  }

  Future _showLogoutDialog(BuildContext context) async {
    final authProvider = Provider.of<AuthProvider>(context, listen: false);
    await showDialog(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text("Logout"),
        content: const Text("Are you sure you want to logout?"),
        actions: [
          TextButton(
            onPressed: () {
              authProvider.logout();
              AuthProvider.userId = 0;
              Navigator.of(ctx).pop(true);
              Navigator.pushReplacement(
                context,
                MaterialPageRoute(builder: (context) => const ProfileScreen()),
              );
            },
            child: const Text("Logout"),
          ),
          TextButton(
            onPressed: () => Navigator.of(ctx).pop(false),
            child: const Text("Cancel"),
          ),
        ],
      ),
    );
  }

  Widget _buildResultView() {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              CircleAvatar(
                backgroundImage: NetworkImage(
                  "${Globals.apiAddress}/images/users/${_user?.filePath}.webp",
                ),
                radius: 40,
              ),
              const SizedBox(width: 16),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      _user?.userName ?? "",
                      style: const TextStyle(
                        fontSize: 18,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                    const SizedBox(height: 4),
                    Row(
                      children: [
                        Text(
                          "${_user?.firstName} ${_user?.lastName}",
                          style: const TextStyle(fontSize: 16),
                        ),
                        if (_user?.publisherVerifiedById != null) ...[
                          const SizedBox(width: 6),
                          const Icon(
                            Icons.verified,
                            color: Colors.green,
                            size: 18,
                          ),
                        ],
                      ],
                    ),
                  ],
                ),
              ),
            ],
          ),
          const SizedBox(height: 20),
          const Text(
            "My Books",
            style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
          ),
          const SizedBox(height: 8),
          SizedBox(
            height: 194,
            child: ListView.builder(
              scrollDirection: Axis.horizontal,
              itemCount: _books?.count ?? 0,
              itemBuilder: (context, index) {
                final book = _books!.resultList[index];
                return Padding(
                  padding: const EdgeInsets.only(right: 8),
                  child: Column(
                    children: [
                      ClipRRect(
                        borderRadius: BorderRadius.circular(6),
                        child: Image.network(
                          "${Globals.apiAddress}/images/books/${book.filePath}.webp",
                          height: 150,
                          width: 100,
                          fit: BoxFit.cover,
                          errorBuilder: (context, error, stackTrace) =>
                              const Icon(Icons.broken_image, size: 100),
                        ),
                      ),
                      const SizedBox(height: 4),
                      SizedBox(
                        width: 100,
                        child: Text(
                          book.title ?? "",
                          textAlign: TextAlign.center,
                          maxLines: 2,
                          overflow: TextOverflow.ellipsis,
                          style: const TextStyle(fontSize: 12),
                        ),
                      ),
                    ],
                  ),
                );
              },
            ),
          ),
          Column(
            children: [
              ListTile(
                leading: const Icon(Icons.account_circle),
                title: const Text("Edit profile"),
                onTap: () {},
              ),
              if (_user?.isEmailVerified == false)
                ListTile(
                  leading: const Icon(Icons.email),
                  title: const Text("Verify Email"),
                  onTap: () {},
                ),
              ListTile(
                leading: const Icon(Icons.favorite),
                title: const Text("Wishlist"),
                onTap: () {},
              ),
              ListTile(
                leading: const Icon(Icons.bookmark),
                title: const Text("Following Publishers"),
                onTap: () {},
              ),
              ListTile(
                leading: const Icon(Icons.account_balance_wallet),
                title: const Text("Stripe Account"),
                onTap: () {},
              ),
              ListTile(
                leading: const Icon(Icons.question_answer),
                title: const Text("Ask Question"),
                onTap: () {},
              ),
              ListTile(
                leading: const Icon(Icons.shopping_cart),
                title: const Text("Purchases"),
                onTap: () {},
              ),
              ListTile(
                leading: const Icon(Icons.logout),
                title: const Text("Logout"),
                onTap: () async {
                  await _showLogoutDialog(context);
                },
              ),
            ],
          ),
        ],
      ),
    );
  }
}
