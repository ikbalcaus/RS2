import "package:ebooks_user/models/search_result.dart";
import "package:ebooks_user/models/wishlist/wishlist.dart";
import "package:ebooks_user/providers/wishlist_provider.dart";
import "package:ebooks_user/screens/master_screen.dart";
import "package:ebooks_user/utils/helpers.dart";
import "package:ebooks_user/widgets/book_card_view.dart";
import "package:flutter/material.dart";
import "package:provider/provider.dart";

class WishlistScreen extends StatefulWidget {
  const WishlistScreen({super.key});

  @override
  State<WishlistScreen> createState() => _BooksScreenState();
}

class _BooksScreenState extends State<WishlistScreen> {
  late WishlistProvider _wishlistProvider;
  SearchResult<Wishlist>? _wishlist;
  bool _isLoading = true;
  int _currentPage = 1;
  final ScrollController _scrollController = ScrollController();

  @override
  void initState() {
    super.initState();
    _wishlistProvider = context.read<WishlistProvider>();
    _fetchBooks();
    _scrollController.addListener(() {
      if (_scrollController.position.pixels >=
          _scrollController.position.maxScrollExtent - 200) {
        if (!_isLoading &&
            (_wishlist?.resultList.length ?? 0) < (_wishlist?.count ?? 0)) {
          _currentPage++;
          _fetchBooks();
        }
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
    Widget content;
    if (_isLoading) {
      content = const Center(child: CircularProgressIndicator());
    } else if (_wishlist?.count == 0) {
      content = const Center(child: Text("Wishlist is empty"));
    } else {
      content = _buildResultView();
    }
    return MasterScreen(showBackButton: true, child: content);
  }

  Future _fetchBooks() async {
    setState(() => _isLoading = true);
    try {
      final wishlist = await _wishlistProvider.getPaged(page: _currentPage);
      if (!mounted) return;
      setState(() {
        _wishlist = wishlist;
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

  Widget _buildResultView() {
    final wishlist = _wishlist?.resultList ?? [];
    return ListView.builder(
      controller: _scrollController,
      padding: const EdgeInsets.all(6),
      itemCount: wishlist.length + (_isLoading ? 1 : 0),
      itemBuilder: (context, index) {
        if (_isLoading && index == wishlist.length) {
          return const Padding(
            padding: EdgeInsets.all(16),
            child: Center(child: CircularProgressIndicator()),
          );
        }
        return BookCardView(
          book: wishlist[index].book!,
          popupActions: {
            "Move to the Top": () async {
              try {
                await _wishlistProvider.patch(wishlist[index].book!.bookId!);
                _fetchBooks();
              } catch (ex) {
                Helpers.showErrorMessage(context, ex);
              }
            },
            "Remove from Wishlist": () async {
              try {
                await _wishlistProvider.delete(wishlist[index].book!.bookId!);
                Helpers.showSuccessMessage(
                  context,
                  "Book is removed to your wishlist",
                );
                _fetchBooks();
              } catch (ex) {
                Helpers.showErrorMessage(context, ex);
              }
            },
          },
        );
      },
    );
  }
}
