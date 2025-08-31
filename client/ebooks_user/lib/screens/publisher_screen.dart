import "package:ebooks_user/models/books/book.dart";
import "package:ebooks_user/models/search_result.dart";
import "package:ebooks_user/models/users/user.dart";
import "package:ebooks_user/providers/auth_provider.dart";
import "package:ebooks_user/providers/books_provider.dart";
import "package:ebooks_user/providers/publisher_follows_provider.dart";
import "package:ebooks_user/providers/users_provider.dart";
import "package:ebooks_user/providers/wishlist_provider.dart";
import "package:ebooks_user/screens/master_screen.dart";
import "package:ebooks_user/utils/globals.dart";
import "package:ebooks_user/utils/helpers.dart";
import "package:ebooks_user/widgets/book_card_view.dart";
import "package:flutter/material.dart";
import "package:provider/provider.dart";

class PublisherScreen extends StatefulWidget {
  final int publisherId;
  const PublisherScreen({super.key, required this.publisherId});

  @override
  State<PublisherScreen> createState() => _PublisherScreenState();
}

class _PublisherScreenState extends State<PublisherScreen> {
  late UsersProvider _usersProvider;
  late BooksProvider _booksProvider;
  late PublisherFollowsProvider _publisherFollowsProvider;
  late WishlistProvider _wishlistProvider;
  User? _user;
  SearchResult<Book>? _books;
  bool _isLoading = true;
  int _currentPage = 1;
  bool _isFollowing = false;
  final ScrollController _scrollController = ScrollController();

  @override
  void initState() {
    super.initState();
    _usersProvider = context.read<UsersProvider>();
    _booksProvider = context.read<BooksProvider>();
    _publisherFollowsProvider = context.read<PublisherFollowsProvider>();
    _wishlistProvider = context.read<WishlistProvider>();
    _fetchUser();
    _fetchBooks();
    _checkisFollowing();
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

  @override
  void dispose() {
    _scrollController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return MasterScreen(
      showBackButton: true,
      child: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : _buildResultView(),
    );
  }

  Future _fetchUser() async {
    setState(() => _isLoading = true);
    try {
      final user = await _usersProvider.getById(widget.publisherId);
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
        filter: {"PublisherId": widget.publisherId},
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

  Future _checkisFollowing() async {
    try {
      await _publisherFollowsProvider.getById(widget.publisherId);
      if (!mounted) return;
      setState(() {
        _isFollowing = true;
      });
    } catch (ex) {}
  }

  Future _toggleFollow() async {
    try {
      _isFollowing
          ? await _publisherFollowsProvider.delete(widget.publisherId)
          : await _publisherFollowsProvider.post(null, widget.publisherId);
      if (!mounted) return;
      setState(() {
        _isFollowing = !_isFollowing;
      });
      Helpers.showSuccessMessage(
        context,
        _isFollowing ? "Publisher is followed" : "Publisher is unfollowed",
      );
    } catch (ex) {
      if (!mounted) return;
      Helpers.showErrorMessage(context, ex);
    }
  }

  Widget _buildResultView() {
    final books = _books?.resultList ?? [];
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Padding(
          padding: const EdgeInsets.only(
            top: 16,
            right: 16,
            bottom: 8,
            left: 16,
          ),
          child: Row(
            children: [
              CircleAvatar(
                radius: 40,
                backgroundColor: Colors.grey[200],
                child: ClipOval(
                  child: Image.network(
                    "${Globals.apiAddress}/images/users/${_user?.filePath}.webp",
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
              if (AuthProvider.isLoggedIn)
                ElevatedButton(
                  onPressed: () async => await _toggleFollow(),
                  child: !_isFollowing
                      ? const Text("Follow")
                      : const Text("Unfollow"),
                ),
            ],
          ),
        ),
        Expanded(
          child: ListView.builder(
            controller: _scrollController,
            padding: const EdgeInsets.all(6),
            itemCount: books.length + (_isLoading ? 1 : 0),
            itemBuilder: (context, index) {
              if (_isLoading && index == books.length) {
                return const Padding(
                  padding: EdgeInsets.all(16),
                  child: Center(child: CircularProgressIndicator()),
                );
              }
              return BookCardView(
                book: books[index],
                popupActions: {
                  "Add to Wishlist": () async {
                    try {
                      await _wishlistProvider.post(null, books[index].bookId);
                      Helpers.showSuccessMessage(
                        context,
                        "Book is added to your wishlist",
                      );
                    } catch (ex) {
                      AuthProvider.isLoggedIn
                          ? Helpers.showErrorMessage(
                              context,
                              "Book is already in your wishlist",
                            )
                          : Helpers.showErrorMessage(
                              context,
                              "You must be logged in",
                            );
                    }
                  },
                },
              );
            },
          ),
        ),
      ],
    );
  }
}
