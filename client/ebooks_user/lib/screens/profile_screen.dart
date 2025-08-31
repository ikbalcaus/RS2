import "dart:ui";
import "package:ebooks_user/models/books/book.dart";
import "package:ebooks_user/models/search_result.dart";
import "package:ebooks_user/models/users/user.dart";
import "package:ebooks_user/providers/auth_provider.dart";
import "package:ebooks_user/providers/books_provider.dart";
import "package:ebooks_user/providers/stripe_provider.dart";
import "package:ebooks_user/providers/theme_provider.dart";
import "package:ebooks_user/providers/users_provider.dart";
import "package:ebooks_user/screens/book_details_screen.dart";
import "package:ebooks_user/screens/edit_book_screen.dart";
import "package:ebooks_user/screens/edit_profile_screen.dart";
import "package:ebooks_user/screens/faq_screen.dart";
import "package:ebooks_user/screens/login_screen.dart";
import "package:ebooks_user/screens/master_screen.dart";
import "package:ebooks_user/screens/followed_publishers_screen.dart";
import "package:ebooks_user/screens/payment_history_screen.dart";
import "package:ebooks_user/screens/wishlist_screen.dart";
import "package:ebooks_user/utils/globals.dart";
import "package:ebooks_user/utils/helpers.dart";
import "package:flutter/material.dart";
import "package:provider/provider.dart";
import "package:url_launcher/url_launcher.dart";

class ProfileScreen extends StatefulWidget {
  const ProfileScreen({super.key});

  @override
  State<ProfileScreen> createState() => _ProfileScreenState();
}

class _ProfileScreenState extends State<ProfileScreen> {
  late UsersProvider _usersProvider;
  late BooksProvider _booksProvider;
  late StripeProvider _stripeProvider;
  User? _user;
  SearchResult<Book>? _books;
  bool _isLoading = true;
  int _currentPage = 1;
  final ScrollController _scrollController = ScrollController();

  @override
  void initState() {
    super.initState();
    if (AuthProvider.isLoggedIn) {
      _usersProvider = context.read<UsersProvider>();
      _booksProvider = context.read<BooksProvider>();
      _stripeProvider = context.read<StripeProvider>();
      _fetchUser();
      _fetchBooks();
      _scrollController.addListener(() {
        if (_scrollController.position.pixels >=
                _scrollController.position.maxScrollExtent - 200 &&
            !_isLoading &&
            (_books?.resultList.length ?? 0) < (_books?.count ?? 0)) {
          _currentPage++;
          _fetchBooks(append: true);
        }
      });
    }
  }

  @override
  void dispose() {
    _scrollController.dispose();
    super.dispose();
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

  Future _fetchUser() async {
    setState(() => _isLoading = true);
    try {
      final user = await _usersProvider.getById(AuthProvider.userId ?? 0);
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

  Future _fetchBooks({bool append = false}) async {
    try {
      final books = await _booksProvider.getPaged(
        page: _currentPage,
        filter: {"PublisherId": AuthProvider.userId},
      );
      if (!mounted) return;
      setState(() {
        if (append && _books != null) {
          _books?.resultList.addAll(books.resultList);
          _books?.count = books.count;
        } else {
          _books = books;
        }
      });
    } catch (ex) {
      if (!mounted) return;
      Helpers.showErrorMessage(context, ex);
    }
  }

  Future _verifyEmailDialog() async {
    String token = "";
    await showDialog(
      context: context,
      builder: (context) {
        return AlertDialog(
          title: const Text("Verify email"),
          content: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              TextField(
                decoration: const InputDecoration(labelText: "Enter token..."),
                onChanged: (value) => token = value,
              ),
              const SizedBox(height: 6),
              TextButton(
                onPressed: () async {
                  try {
                    await _usersProvider.verifyEmail(_user!.userId!, null);
                    Helpers.showSuccessMessage(
                      context,
                      "Verification email has been sent",
                    );
                  } catch (ex) {
                    Helpers.showErrorMessage(context, ex);
                  }
                },
                child: const Text("Send Verification Email"),
              ),
            ],
          ),
          actions: [
            TextButton(
              onPressed: () async {
                if (token.trim().isNotEmpty) {
                  try {
                    await _usersProvider.verifyEmail(_user!.userId!, token);
                    await _fetchUser();
                    if (context.mounted) {
                      Navigator.pop(context);
                      Helpers.showSuccessMessage(context);
                    }
                  } catch (ex) {
                    Helpers.showErrorMessage(context, ex);
                  }
                }
              },
              child: const Text("Verify"),
            ),
            TextButton(
              onPressed: () => Navigator.pop(context),
              child: const Text("Cancel"),
            ),
          ],
        );
      },
    );
  }

  Future _openStripeAccount() async {
    try {
      var accountLink = await _stripeProvider.getStripeAccountLink();
      final uri = Uri.parse(accountLink.url);
      if (await canLaunchUrl(uri)) {
        await launchUrl(uri, mode: LaunchMode.externalApplication);
      } else {
        Helpers.showErrorMessage(
          context,
          "Cannot opet an URL: ${uri.toString()}",
        );
      }
    } catch (ex) {
      Helpers.showErrorMessage(context, ex);
    }
  }

  void _showSelectModeDialog() {
    final themeProvider = Provider.of<ThemeProvider>(context, listen: false);
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text("Choose Theme"),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            RadioListTile(
              title: const Text("Light"),
              value: ThemeMode.light,
              groupValue: themeProvider.themeMode,
              onChanged: (value) {
                themeProvider.setThemeMode(value!);
                Navigator.of(context).pop();
              },
            ),
            RadioListTile(
              title: const Text("Dark"),
              value: ThemeMode.dark,
              groupValue: themeProvider.themeMode,
              onChanged: (value) {
                themeProvider.setThemeMode(value!);
                Navigator.of(context).pop();
              },
            ),
            RadioListTile(
              title: const Text("System"),
              value: ThemeMode.system,
              groupValue: themeProvider.themeMode,
              onChanged: (value) {
                themeProvider.setThemeMode(value!);
                Navigator.of(context).pop();
              },
            ),
          ],
        ),
      ),
    );
  }

  Future _showLogoutDialog() async {
    final authProvider = Provider.of<AuthProvider>(context, listen: false);
    await showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text("Logout"),
        content: const Text("Are you sure you want to logout?"),
        actions: [
          TextButton(
            onPressed: () {
              authProvider.logout();
              AuthProvider.userId = 0;
              Navigator.pop(context);
              Navigator.pushAndRemoveUntil(
                context,
                MaterialPageRoute(builder: (_) => const ProfileScreen()),
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

  Widget _buildResultView() {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              CircleAvatar(
                radius: 40,
                backgroundColor: Colors.grey[200],
                child: ClipOval(
                  child: Image.network(
                    "${Globals.apiAddress}/images/users/${_user?.filePath}.webp?t=${DateTime.now().millisecondsSinceEpoch}",
                    width: 80,
                    height: 80,
                    fit: BoxFit.cover,
                    errorBuilder: (context, error, stackTrace) {
                      return Icon(
                        Icons.person,
                        size: 40,
                        color: Colors.grey[800],
                      );
                    },
                  ),
                ),
              ),
              const SizedBox(width: 12),
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
          const SizedBox(height: 24),
          if (_books?.count != 0)
            Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const Text(
                  "My Books",
                  style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
                ),
                const SizedBox(height: 8),
                SizedBox(
                  height: 210,
                  child: ScrollConfiguration(
                    behavior: ScrollConfiguration.of(context).copyWith(
                      dragDevices: {
                        PointerDeviceKind.touch,
                        PointerDeviceKind.mouse,
                      },
                    ),
                    child: ListView.builder(
                      controller: _scrollController,
                      scrollDirection: Axis.horizontal,
                      itemCount: _books?.resultList.length,
                      itemBuilder: (context, index) {
                        final book = _books?.resultList[index];
                        return Padding(
                          padding: const EdgeInsets.only(right: 8),
                          child: InkWell(
                            onTap: () => Navigator.push(
                              context,
                              MaterialPageRoute(
                                builder: (context) =>
                                    BookDetailsScreen(bookId: book!.bookId!),
                              ),
                            ),
                            child: Column(
                              children: [
                                ClipRRect(
                                  borderRadius: BorderRadius.circular(4),
                                  child: Image.network(
                                    "${Globals.apiAddress}/images/books/${book?.filePath}.webp?t=${DateTime.now().millisecondsSinceEpoch}",
                                    height: 165,
                                    width: 110,
                                    fit: BoxFit.cover,
                                    errorBuilder:
                                        (context, error, stackTrace) =>
                                            SizedBox(
                                              height: 165,
                                              width: 110,
                                              child: FittedBox(
                                                fit: BoxFit.contain,
                                                child: const Icon(Icons.book),
                                              ),
                                            ),
                                  ),
                                ),
                                const SizedBox(height: 4),
                                SizedBox(
                                  width: 100,
                                  child: Text(
                                    book?.title ?? "",
                                    textAlign: TextAlign.center,
                                    maxLines: 2,
                                    overflow: TextOverflow.ellipsis,
                                    style: const TextStyle(
                                      fontSize: 12,
                                      fontWeight: FontWeight.w500,
                                    ),
                                  ),
                                ),
                              ],
                            ),
                          ),
                        );
                      },
                    ),
                  ),
                ),
              ],
            ),
          Column(
            children: [
              const Divider(),
              ListTile(
                leading: const Icon(Icons.account_circle),
                title: const Text("Edit Profile"),
                onTap: () => Navigator.push(
                  context,
                  MaterialPageRoute(
                    builder: (context) => const EditProfileScreen(),
                  ),
                ),
              ),
              if (_user?.isEmailVerified == false)
                ListTile(
                  leading: const Icon(Icons.email),
                  title: const Text("Verify Email"),
                  onTap: () async => await _verifyEmailDialog(),
                ),
              ListTile(
                leading: const Icon(Icons.book),
                title: const Text("Add New Book"),
                onTap: () => Navigator.push(
                  context,
                  MaterialPageRoute(
                    builder: (context) => const EditBookScreen(),
                  ),
                ),
              ),
              ListTile(
                leading: const Icon(Icons.favorite),
                title: const Text("Wishlist"),
                onTap: () => Navigator.push(
                  context,
                  MaterialPageRoute(
                    builder: (context) => const WishlistScreen(),
                  ),
                ),
              ),
              ListTile(
                leading: const Icon(Icons.bookmark),
                title: const Text("Followed Publishers"),
                onTap: () => Navigator.push(
                  context,
                  MaterialPageRoute(
                    builder: (context) => const FollowedPublishersScreen(),
                  ),
                ),
              ),
              const Divider(),
              ListTile(
                leading: const Icon(Icons.account_balance_wallet),
                title: const Text("Stripe Account"),
                trailing: Icon(Icons.chevron_right_rounded),
                onTap: () async => await _openStripeAccount(),
              ),
              ListTile(
                leading: const Icon(Icons.color_lens),
                title: const Text("Change Theme"),
                onTap: () => _showSelectModeDialog(),
              ),
              ListTile(
                leading: const Icon(Icons.payment),
                title: const Text("Payment History"),
                onTap: () => Navigator.push(
                  context,
                  MaterialPageRoute(
                    builder: (context) =>
                        const PaymentHistoryScreen(publisherId: 1),
                  ),
                ),
              ),
              ListTile(
                leading: const Icon(Icons.question_mark),
                title: const Text("Help and Support"),
                onTap: () => Navigator.push(
                  context,
                  MaterialPageRoute(builder: (context) => const FaqScreen()),
                ),
              ),
              const Divider(),
              ListTile(
                leading: const Icon(Icons.logout),
                title: const Text("Logout"),
                onTap: () async => await _showLogoutDialog(),
              ),
            ],
          ),
        ],
      ),
    );
  }
}
