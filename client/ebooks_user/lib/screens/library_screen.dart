import "package:ebooks_user/models/access_rights/access_right.dart";
import "package:ebooks_user/models/search_result.dart";
import "package:ebooks_user/providers/access_rights_provider.dart";
import "package:ebooks_user/providers/auth_provider.dart";
import "package:ebooks_user/screens/master_screen.dart";
import "package:ebooks_user/utils/globals.dart";
import "package:ebooks_user/utils/helpers.dart";
import "package:ebooks_user/widgets/not_logged_in_view.dart";
import "package:flutter/material.dart";
import "package:provider/provider.dart";

class LibraryScreen extends StatefulWidget {
  const LibraryScreen({super.key});

  @override
  State<LibraryScreen> createState() => _LibraryScreenState();
}

class _LibraryScreenState extends State<LibraryScreen> {
  late AccessRightsProvider _accessRightsProvider;
  SearchResult<AccessRight>? _accessRights;
  bool _isLoading = true;
  int _currentPage = 1;
  final ScrollController _scrollController = ScrollController();

  @override
  void initState() {
    super.initState();
    if (AuthProvider.isLoggedIn) {
      _accessRightsProvider = context.read<AccessRightsProvider>();
      _fetchAccessRights();
      _scrollController.addListener(() {
        if (_scrollController.position.pixels >=
            _scrollController.position.maxScrollExtent - 200) {
          if (!_isLoading &&
              (_accessRights?.resultList.length ?? 0) <
                  (_accessRights?.count ?? 0)) {
            _currentPage++;
            _fetchAccessRights(append: true);
          }
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
    Widget content;
    if (!AuthProvider.isLoggedIn) {
      content = Center(child: NotLoggedInView());
    } else if (_isLoading) {
      content = const Center(child: CircularProgressIndicator());
    } else if (_accessRights?.count == 0) {
      content = const Center(
        child: Text("You don't have any books in your library"),
      );
    } else {
      content = _buildResultView();
    }
    return MasterScreen(child: content);
  }

  Future _fetchAccessRights({bool append = false}) async {
    setState(() => _isLoading = true);
    try {
      final books = await _accessRightsProvider.getPaged(page: _currentPage);
      if (!mounted) return;
      setState(() {
        if (append && _accessRights != null) {
          _accessRights?.resultList.addAll(books.resultList);
          _accessRights?.count = books.count;
        } else {
          _accessRights = books;
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

  Widget _buildResultView() {
    final accessRight = _accessRights?.resultList ?? [];
    return GridView.builder(
      controller: _scrollController,
      padding: const EdgeInsets.symmetric(vertical: 12, horizontal: 6),
      gridDelegate: const SliverGridDelegateWithMaxCrossAxisExtent(
        maxCrossAxisExtent: 200,
        mainAxisSpacing: 6,
        crossAxisSpacing: 6,
        childAspectRatio: 200 / 300,
      ),
      itemCount: accessRight.length + (_isLoading ? 1 : 0),
      itemBuilder: (context, index) {
        if (_isLoading && index == accessRight.length) {
          return const Padding(
            padding: EdgeInsets.all(16),
            child: Center(child: CircularProgressIndicator()),
          );
        }
        return InkWell(
          onTap: () {},
          child: SizedBox(
            child: ClipRRect(
              borderRadius: BorderRadius.circular(2),
              child: Stack(
                children: [
                  Image.network(
                    "${Globals.apiAddress}/images/books/${_accessRights?.resultList[index].book?.filePath}.webp",
                    height: double.infinity,
                    fit: BoxFit.cover,
                    errorBuilder: (context, error, stackTrace) =>
                        const Icon(Icons.broken_image, size: 100),
                  ),
                  Positioned(
                    top: 4,
                    right: 4,
                    child: GestureDetector(
                      onTap: () async {
                        try {
                          await _accessRightsProvider.toggleFavorite(
                            _accessRights!.resultList[index].book!.bookId!,
                          );
                          await _fetchAccessRights();
                        } catch (ex) {
                          Helpers.showErrorMessage(context, ex);
                        }
                      },
                      child: Icon(
                        _accessRights?.resultList[index].isFavorite == true
                            ? Icons.favorite
                            : Icons.favorite_border,
                        color:
                            _accessRights?.resultList[index].isFavorite == true
                            ? Globals.backgroundColor
                            : Colors.white,
                        size: 24,
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ),
        );
      },
    );
  }
}
