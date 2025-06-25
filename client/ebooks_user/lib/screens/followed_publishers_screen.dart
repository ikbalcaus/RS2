import "package:ebooks_user/models/books/book.dart";
import "package:ebooks_user/models/publisher_follow/publisher_follow.dart";
import "package:ebooks_user/models/search_result.dart";
import "package:ebooks_user/providers/books_provider.dart";
import "package:ebooks_user/providers/publisher_follows_provider.dart";
import "package:ebooks_user/screens/master_screen.dart";
import "package:ebooks_user/screens/publisher_screen.dart";
import "package:ebooks_user/utils/globals.dart";
import "package:ebooks_user/utils/helpers.dart";
import "package:ebooks_user/widgets/book_card_view.dart";
import "package:flutter/material.dart";
import "package:provider/provider.dart";

class FollowedPublishersScreen extends StatefulWidget {
  const FollowedPublishersScreen({super.key});

  @override
  State<FollowedPublishersScreen> createState() => _FollowedPublishersState();
}

class _FollowedPublishersState extends State<FollowedPublishersScreen> {
  late BooksProvider _booksProvider;
  late PublisherFollowsProvider _publisherFollowsProvider;
  SearchResult<Book>? _books;
  SearchResult<PublisherFollow>? _publishers;
  bool _isLoading = true;
  int _currentPageBooks = 1;
  final int _currentPagePublishers = 1;
  Map<String, dynamic> _currentFilter = {};
  final ScrollController _scrollControllerHorizontal = ScrollController();
  final ScrollController _scrollControllerVertical = ScrollController();

  @override
  void initState() {
    super.initState();
    _booksProvider = context.read<BooksProvider>();
    _publisherFollowsProvider = context.read<PublisherFollowsProvider>();
    _currentFilter = {
      "Status": "Approved",
      "IsDeleted": "Not deleted",
      "FollowingPublishersOnly": true,
    };
    _fetchBooks();
    _fetchPublishers();
    _scrollControllerVertical.addListener(() {
      if (_scrollControllerVertical.position.pixels >=
          _scrollControllerVertical.position.maxScrollExtent - 200) {
        if (!_isLoading &&
            (_books?.resultList.length ?? 0) < (_books?.count ?? 0)) {
          _currentPageBooks++;
          _fetchBooks(append: true);
        }
      }
    });
    _scrollControllerHorizontal.addListener(() {
      if (_scrollControllerHorizontal.position.pixels >=
              _scrollControllerHorizontal.position.maxScrollExtent - 200 &&
          !_isLoading &&
          (_publishers?.resultList.length ?? 0) < (_publishers?.count ?? 0)) {
        _currentPageBooks++;
        _fetchPublishers(append: true);
      }
    });
  }

  @override
  void dispose() {
    _scrollControllerHorizontal.dispose();
    _scrollControllerVertical.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    Widget content;
    if (_isLoading) {
      content = const Center(child: CircularProgressIndicator());
    } else if (_publishers?.count == 0) {
      content = const Center(child: Text("You don't follow any publisher"));
    } else {
      content = _buildResultView();
    }
    return MasterScreen(showBackButton: true, child: content);
  }

  Future _fetchBooks({bool append = false}) async {
    setState(() => _isLoading = true);
    try {
      final books = await _booksProvider.getPaged(
        page: _currentPageBooks,
        filter: _currentFilter,
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
      WidgetsBinding.instance.addPostFrameCallback((_) {
        if (!mounted) return;
        Helpers.showErrorMessage(context, ex);
      });
    } finally {
      if (!mounted) return;
      setState(() => _isLoading = false);
    }
  }

  Future _fetchPublishers({bool append = false}) async {
    setState(() => _isLoading = true);
    try {
      final publishers = await _publisherFollowsProvider.getPaged(
        page: _currentPagePublishers,
      );
      if (!mounted) return;
      setState(() {
        if (append && _publishers != null) {
          _publishers?.resultList.addAll(publishers.resultList);
          _publishers?.count = publishers.count;
        } else {
          _publishers = publishers;
        }
      });
    } catch (ex) {
      if (!mounted) return;
      Helpers.showErrorMessage(context, ex);
    } finally {
      if (!mounted) return;
      setState(() => _isLoading = false);
    }
  }

  Widget _buildResultView() {
    final books = _books?.resultList ?? [];
    final publishers = _publishers?.resultList ?? [];
    return Column(
      children: [
        Padding(
          padding: const EdgeInsets.only(top: 16),
          child: SizedBox(
            height: 110,
            child: ListView.builder(
              controller: _scrollControllerHorizontal,
              scrollDirection: Axis.horizontal,
              itemCount: _publishers?.resultList.length ?? 0,
              itemBuilder: (context, index) {
                final publisher = _publishers!.resultList[index].publisher;
                return Padding(
                  padding: const EdgeInsets.symmetric(horizontal: 8),
                  child: InkWell(
                    onTap: () => Navigator.push(
                      context,
                      MaterialPageRoute(
                        builder: (context) => PublisherScreen(
                          publisherId: _publishers!.resultList[index].userId!,
                        ),
                      ),
                    ),
                    child: Column(
                      children: [
                        CircleAvatar(
                          radius: 40,
                          backgroundColor: Colors.grey[200],
                          child: ClipOval(
                            child: Image.network(
                              "${Globals.apiAddress}/images/users/${publishers[index].publisher?.filePath}.webp",
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
                        const SizedBox(height: 4),
                        Row(
                          children: [
                            SizedBox(
                              child: Text(
                                publisher?.userName ?? "",
                                style: const TextStyle(
                                  fontSize: 12,
                                  fontWeight: FontWeight.w500,
                                ),
                                overflow: TextOverflow.ellipsis,
                                textAlign: TextAlign.center,
                              ),
                            ),
                            if (publisher?.publisherVerifiedById != null) ...[
                              const SizedBox(width: 2),
                              const Icon(
                                Icons.verified,
                                color: Colors.green,
                                size: 13,
                              ),
                            ],
                          ],
                        ),
                      ],
                    ),
                  ),
                );
              },
            ),
          ),
        ),
        Expanded(
          child: ListView.builder(
            controller: _scrollControllerVertical,
            padding: const EdgeInsets.only(right: 6, bottom: 6, left: 6),
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
                  "Unfollow Publisher": () async {
                    try {
                      await _publisherFollowsProvider.delete(
                        books[index].publisher!.userId!,
                      );
                      Helpers.showSuccessMessage(
                        context,
                        "You are now not following ${books[index].publisher?.userName}",
                      );
                      _fetchPublishers();
                      _fetchBooks();
                    } catch (ex) {
                      Helpers.showErrorMessage(context, ex);
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
