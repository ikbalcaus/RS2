import "dart:ui";
import "package:easy_localization/easy_localization.dart";
import "package:ebooks_user/models/books/book.dart";
import "package:ebooks_user/models/search_result.dart";
import "package:ebooks_user/models/users/user.dart";
import "package:ebooks_user/providers/auth_provider.dart";
import "package:ebooks_user/providers/books_provider.dart";
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
  int _currentPage = 1;
  final ScrollController _scrollController = ScrollController();

  @override
  void initState() {
    super.initState();
    if (AuthProvider.isLoggedIn) {
      _usersProvider = context.read<UsersProvider>();
      _booksProvider = context.read<BooksProvider>();
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
          title: Text("Verify email".tr()),
          content: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              TextField(
                decoration: InputDecoration(labelText: "Enter token...".tr()),
                onChanged: (value) => token = value,
              ),
              const SizedBox(height: 6),
              TextButton(
                onPressed: () async {
                  try {
                    await _usersProvider.verifyEmail(_user!.userId!, null);
                    Helpers.showSuccessMessage(
                      context,
                      "Verification email has been sent".tr(),
                    );
                  } catch (ex) {
                    Helpers.showErrorMessage(context, ex);
                  }
                },
                child: Text("Send Verification Email".tr()),
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
              child: Text("Verify".tr()),
            ),
            TextButton(
              onPressed: () => Navigator.pop(context),
              child: Text("Cancel".tr()),
            ),
          ],
        );
      },
    );
  }

  void _showSelectModeDialog() {
    final themeProvider = Provider.of<ThemeProvider>(context, listen: false);
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: Text("Choose Theme".tr()),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            RadioListTile(
              title: Text("Light".tr()),
              value: ThemeMode.light,
              groupValue: themeProvider.themeMode,
              onChanged: (value) {
                themeProvider.setThemeMode(value!);
                Navigator.of(context).pop();
              },
            ),
            RadioListTile(
              title: Text("Dark".tr()),
              value: ThemeMode.dark,
              groupValue: themeProvider.themeMode,
              onChanged: (value) {
                themeProvider.setThemeMode(value!);
                Navigator.of(context).pop();
              },
            ),
            RadioListTile(
              title: Text("System".tr()),
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

  void _showSelectLanguageDialog() {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: Text("Select Language".tr()),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            RadioListTile<Locale>(
              title: Text("English".tr()),
              value: const Locale('en'),
              groupValue: context.locale,
              onChanged: (locale) {
                context.setLocale(locale!);
                Navigator.of(context).pop();
              },
            ),
            RadioListTile<Locale>(
              title: Text("Bosnian".tr()),
              value: const Locale('bs'),
              groupValue: context.locale,
              onChanged: (locale) {
                context.setLocale(locale!);
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
        title: Text("Logout".tr()),
        content: Text("Are you sure you want to logout?".tr()),
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
            child: Text("Logout".tr()),
          ),
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: Text("Cancel".tr()),
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
                Text(
                  "My Books".tr(),
                  style: const TextStyle(
                    fontSize: 18,
                    fontWeight: FontWeight.bold,
                  ),
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
                title: Text("Edit Profile".tr()),
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
                  title: Text("Verify Email".tr()),
                  onTap: () async => await _verifyEmailDialog(),
                ),
              ListTile(
                leading: const Icon(Icons.book),
                title: Text("Add new Book".tr()),
                onTap: () => Navigator.push(
                  context,
                  MaterialPageRoute(
                    builder: (context) => const EditBookScreen(),
                  ),
                ),
              ),
              ListTile(
                leading: const Icon(Icons.favorite),
                title: Text("Wishlist".tr()),
                onTap: () => Navigator.push(
                  context,
                  MaterialPageRoute(
                    builder: (context) => const WishlistScreen(),
                  ),
                ),
              ),
              ListTile(
                leading: const Icon(Icons.bookmark),
                title: Text("Followed Publishers".tr()),
                onTap: () => Navigator.push(
                  context,
                  MaterialPageRoute(
                    builder: (context) => const FollowedPublishersScreen(),
                  ),
                ),
              ),
              const Divider(),
              ListTile(
                leading: const Icon(Icons.color_lens),
                title: Text("Change Theme".tr()),
                onTap: () => _showSelectModeDialog(),
              ),
              ListTile(
                leading: const Icon(Icons.language),
                title: Text("Change Language".tr()),
                onTap: () => _showSelectLanguageDialog(),
              ),
              ListTile(
                leading: const Icon(Icons.payment),
                title: Text("Payment History".tr()),
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
                title: Text("Help and Support".tr()),
                onTap: () => Navigator.push(
                  context,
                  MaterialPageRoute(builder: (context) => const FaqScreen()),
                ),
              ),
              const Divider(),
              ListTile(
                leading: const Icon(Icons.logout),
                title: Text("Logout".tr()),
                onTap: () async => await _showLogoutDialog(),
              ),
            ],
          ),
        ],
      ),
    );
  }
}
