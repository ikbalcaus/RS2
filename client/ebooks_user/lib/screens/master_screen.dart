import "package:ebooks_user/providers/theme_provider.dart";
import "package:ebooks_user/screens/books_screen.dart";
import "package:ebooks_user/screens/library_screen.dart";
import "package:ebooks_user/screens/notifications_screen.dart";
import "package:ebooks_user/screens/profile_screen.dart";
import "package:ebooks_user/utils/globals.dart";
import "package:flutter/material.dart";
import "package:provider/provider.dart";

class MasterScreen extends StatefulWidget {
  final Widget child;
  final bool showBackButton;
  final TextEditingController? searchController;
  final VoidCallback? onSearch;
  final VoidCallback? onFilterPressed;
  final VoidCallback? onBookmarkClicked;
  final Map<String, VoidCallback?>? popupActions;

  const MasterScreen({
    super.key,
    required this.child,
    this.showBackButton = false,
    this.searchController,
    this.onSearch,
    this.onFilterPressed,
    this.onBookmarkClicked,
    this.popupActions,
  });

  @override
  State<MasterScreen> createState() => _MasterScreenState();
}

class _MasterScreenState extends State<MasterScreen> {
  bool _showSearch = false;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        automaticallyImplyLeading: false,
        scrolledUnderElevation: 0,
        leading: widget.showBackButton
            ? BackButton(onPressed: () => Navigator.pop(context))
            : null,
        title: Row(
          children: [
            Image.asset("assets/images/logo.png", height: 40),
            const SizedBox(width: 8),
            const Text(
              "E-Books",
              style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
            ),
          ],
        ),
        actions: [
          Padding(
            padding: const EdgeInsets.only(right: 10),
            child: Row(
              children: [
                if (widget.searchController != null)
                  IconButton(
                    icon: _showSearch
                        ? const Icon(Icons.close)
                        : const Icon(Icons.search),
                    onPressed: () {
                      setState(() => _showSearch = !_showSearch);
                    },
                  ),
                if (widget.onBookmarkClicked != null)
                  IconButton(
                    icon: const Icon(Icons.bookmark),
                    tooltip: "Save reading progress",
                    onPressed: () async {
                      widget.onBookmarkClicked?.call();
                    },
                  ),
                if (widget.popupActions != null)
                  PopupMenuButton<String>(
                    icon: const Icon(Icons.more_vert),
                    onSelected: (String title) {
                      final callback = widget.popupActions?[title];
                      if (callback != null) {
                        Future.microtask(() => callback());
                      }
                    },
                    itemBuilder: (context) {
                      return widget.popupActions?.entries.map((entry) {
                            return PopupMenuItem(
                              value: entry.key,
                              child: Text(entry.key),
                            );
                          }).toList() ??
                          [];
                    },
                  ),
              ],
            ),
          ),
        ],
      ),
      body: Column(
        children: [
          if (widget.searchController != null && _showSearch)
            Container(
              color: Globals.backgroundColor,
              padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
              child: Row(
                children: [
                  Expanded(
                    child: TextField(
                      controller: widget.searchController,
                      cursorColor: Colors.white70,
                      decoration: InputDecoration(
                        hintText: "Search...",
                        hintStyle: const TextStyle(color: Colors.white70),
                        contentPadding: const EdgeInsets.symmetric(
                          horizontal: 12,
                        ),
                        enabledBorder: const UnderlineInputBorder(
                          borderSide: BorderSide(color: Colors.white),
                        ),
                        focusedBorder: const UnderlineInputBorder(
                          borderSide: BorderSide(
                            color: Colors.white,
                            width: 1.5,
                          ),
                        ),
                      ),
                      style: const TextStyle(color: Colors.white),
                      onSubmitted: (_) => widget.onSearch?.call(),
                    ),
                  ),
                  const SizedBox(width: 8),
                  IconButton(
                    onPressed: () => widget.onSearch?.call(),
                    icon: const Icon(Icons.search, color: Colors.white),
                  ),
                  IconButton(
                    onPressed: () => widget.onFilterPressed?.call(),
                    icon: const Icon(
                      Icons.filter_alt_outlined,
                      color: Colors.white,
                    ),
                  ),
                ],
              ),
            ),
          Expanded(child: widget.child),
        ],
      ),
      bottomNavigationBar: BottomNavigationBar(
        type: BottomNavigationBarType.fixed,
        currentIndex: Globals.pageIndex,
        onTap: (index) {
          Globals.pageIndex = index;
          switch (index) {
            case 0:
              Navigator.pushAndRemoveUntil(
                context,
                MaterialPageRoute(builder: (_) => const BooksScreen()),
                (_) => false,
              );
              break;
            case 1:
              Navigator.pushAndRemoveUntil(
                context,
                MaterialPageRoute(builder: (_) => const LibraryScreen()),
                (_) => false,
              );
              break;
            case 2:
              Navigator.pushAndRemoveUntil(
                context,
                MaterialPageRoute(builder: (_) => const NotificationsScreen()),
                (_) => false,
              );
              break;
            case 3:
              Navigator.pushAndRemoveUntil(
                context,
                MaterialPageRoute(builder: (_) => const ProfileScreen()),
                (_) => false,
              );
              break;
          }
        },
        selectedItemColor: Globals.backgroundColor,
        unselectedItemColor: (() {
          final themeMode = Provider.of<ThemeProvider>(context).themeMode;
          if (themeMode == ThemeMode.light) {
            return Color.lerp(Colors.black54, Colors.black87, 0.9);
          } else if (themeMode == ThemeMode.dark) {
            return Colors.white70;
          } else {
            final brightness = MediaQuery.of(context).platformBrightness;
            return brightness == Brightness.dark ? Colors.white : Colors.black;
          }
        })(),
        items: const [
          BottomNavigationBarItem(icon: Icon(Icons.book_sharp), label: "Books"),
          BottomNavigationBarItem(
            icon: Icon(Icons.library_books_sharp),
            label: "Library",
          ),
          BottomNavigationBarItem(
            icon: Icon(Icons.notifications),
            label: "Notifications",
          ),
          BottomNavigationBarItem(icon: Icon(Icons.person), label: "Profile"),
        ],
      ),
    );
  }
}
