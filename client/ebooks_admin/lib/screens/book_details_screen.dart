import "dart:convert";
import "dart:io";
import "package:dio/dio.dart";
import "package:ebooks_admin/models/books/book.dart";
import "package:ebooks_admin/providers/auth_provider.dart";
import "package:ebooks_admin/providers/books_provider.dart";
import "package:ebooks_admin/screens/authors_screen.dart";
import "package:ebooks_admin/screens/books_screen.dart";
import "package:ebooks_admin/screens/genres_screen.dart";
import "package:ebooks_admin/screens/languages_screen.dart";
import "package:ebooks_admin/screens/master_screen.dart";
import "package:ebooks_admin/screens/users_screen.dart";
import "package:ebooks_admin/utils/globals.dart";
import "package:ebooks_admin/utils/helpers.dart";
import "package:flutter/gestures.dart";
import "package:flutter/material.dart";
import "package:path/path.dart" as p;
import "package:provider/provider.dart";
import "package:url_launcher/url_launcher.dart";

class BookDetailsScreen extends StatefulWidget {
  final int bookId;
  const BookDetailsScreen({super.key, required this.bookId});

  @override
  State<BookDetailsScreen> createState() => _BookDetailsScreenState();
}

class _BookDetailsScreenState extends State<BookDetailsScreen> {
  late BooksProvider _booksProvider;
  Book? _book;
  final List<String> _allowedActions = [];
  bool _isLoading = true;
  double? _downloadProgress;

  @override
  void initState() {
    super.initState();
    _booksProvider = context.read<BooksProvider>();
    _fetchBook();
    _fetchAllowedActions();
  }

  @override
  Widget build(BuildContext context) {
    if (_isLoading) {
      return MasterScreen(
        child: const Center(child: CircularProgressIndicator()),
      );
    }
    return MasterScreen(child: _buildResultView());
  }

  Future _fetchBook() async {
    setState(() => _isLoading = true);
    try {
      final book = await _booksProvider.getById(widget.bookId);
      if (!mounted) return;
      setState(() => _book = book);
    } catch (ex) {
      if (!mounted) return;
      Helpers.showErrorMessage(context, ex);
    } finally {
      if (!mounted) return;
      setState(() => _isLoading = false);
    }
  }

  Future _fetchAllowedActions() async {
    try {
      final allowedActions = await _booksProvider.getAllowedActions(
        widget.bookId,
      );
      if (!mounted) return;
      setState(() {
        _allowedActions.clear();
        _allowedActions.addAll(allowedActions ?? []);
      });
    } catch (ex) {
      if (!mounted) return;
      Helpers.showErrorMessage(context, ex);
    } finally {
      if (!mounted) return;
    }
  }

  Future _openUrl(String url) async {
    final uri = Uri.parse(url);
    if (await canLaunchUrl(uri)) {
      await launchUrl(uri);
    } else {
      Helpers.showErrorMessage(context, "Cannot open URL: $url");
    }
  }

  Future _getDesktopPath() async {
    final home =
        Platform.environment["USERPROFILE"] ?? Platform.environment["HOME"];
    if (home == null) {
      Helpers.showErrorMessage(
        context,
        "An error occurred while downloading a book",
      );
      return;
    }
    final desktopPath = p.join(home, "Desktop");
    return desktopPath;
  }

  Future _downloadBookFile(int bookId) async {
    try {
      final dio = Dio();
      String username = AuthProvider.email ?? "";
      String password = AuthProvider.password ?? "";
      dio.options.headers["Authorization"] =
          "Basic ${base64Encode(utf8.encode("$username:$password"))}";
      final url = "${Globals.apiAddress}/books/$bookId/book-file";
      final desktopPath = await _getDesktopPath();
      final savePath = p.join(desktopPath, "${_book?.filePath}.pdf");
      final response = await dio.download(
        url,
        savePath,
        onReceiveProgress: (received, total) {
          if (total != -1) {
            setState(() => _downloadProgress = received / total);
          }
        },
      );
      setState(() => _downloadProgress = null);
      if (response.statusCode == 200) {
        Helpers.showSuccessMessage(
          context,
          "Book is successfully downloaded on your desktop",
        );
      } else {
        Helpers.showErrorMessage(
          context,
          "An error occurred while downloading a book",
        );
      }
    } catch (ex) {
      Helpers.showErrorMessage(context, ex);
    }
  }

  Future _approveBookDialog() async {
    await showDialog(
      context: context,
      builder: (context) {
        return AlertDialog(
          title: const Text("Confirm approve"),
          actions: [
            TextButton(
              onPressed: () async {
                Navigator.pop(context);
                await Future.delayed(const Duration(milliseconds: 250));
                try {
                  await _booksProvider.approveBook(widget.bookId);
                  Helpers.showSuccessMessage(
                    context,
                    "A book is successfully approved",
                  );
                  await _fetchBook();
                  await _fetchAllowedActions();
                } catch (ex) {
                  Helpers.showErrorMessage(context, ex);
                }
              },
              child: const Text("Approve"),
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

  Future _rejectBookDialog() async {
    final TextEditingController dialogController = TextEditingController();
    await showDialog(
      context: context,
      builder: (context) {
        return AlertDialog(
          title: const Text("Confirm reject"),
          content: TextField(
            controller: dialogController,
            decoration: const InputDecoration(
              labelText: "Enter reason for rejecting...",
            ),
          ),
          actions: [
            TextButton(
              onPressed: () async {
                final dialogText = dialogController.text.trim();
                if (dialogText.isNotEmpty) {
                  Navigator.pop(context);
                  await Future.delayed(const Duration(milliseconds: 250));
                  try {
                    await _booksProvider.rejectBook(widget.bookId, dialogText);
                    Helpers.showSuccessMessage(
                      context,
                      "Book is successfully rejected",
                    );
                    await _fetchBook();
                    await _fetchAllowedActions();
                  } catch (ex) {
                    Helpers.showErrorMessage(context, ex);
                  }
                }
              },
              child: const Text("Reject"),
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

  Future _deleteBookDialog(BuildContext context) async {
    await showDialog(
      context: context,
      builder: (context) {
        String reason = "";
        return AlertDialog(
          title: const Text("Confirm delete"),
          content: TextField(
            decoration: const InputDecoration(
              labelText: "Enter reason for deleting...",
            ),
            onChanged: (value) => reason = value,
          ),
          actions: [
            TextButton(
              onPressed: () async {
                if (reason.trim().isNotEmpty) {
                  Navigator.pop(context);
                  await Future.delayed(const Duration(milliseconds: 250));
                  try {
                    await _booksProvider.adminDelete(widget.bookId, reason);
                    Helpers.showSuccessMessage(
                      context,
                      "User is successfully deleted",
                    );
                    await _fetchBook();
                  } catch (ex) {
                    Helpers.showErrorMessage(context, ex);
                  }
                }
              },
              child: const Text("Delete"),
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

  Future _undoDeleteBookDialog(BuildContext context) async {
    await showDialog(
      context: context,
      builder: (context) {
        return AlertDialog(
          title: const Text("Confirm undo delete"),
          actions: [
            TextButton(
              onPressed: () async {
                Navigator.pop(context);
                await Future.delayed(const Duration(milliseconds: 250));
                try {
                  await _booksProvider.adminDelete(widget.bookId, null);
                  Helpers.showSuccessMessage(
                    context,
                    "User is successfully undo deleted",
                  );
                  await _fetchBook();
                } catch (ex) {
                  Helpers.showErrorMessage(context, ex);
                }
              },
              child: const Text("Undo delete"),
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

  Widget _buildResultView() {
    return Align(
      alignment: Alignment.topCenter,
      child: SingleChildScrollView(
        padding: const EdgeInsets.symmetric(vertical: 32, horizontal: 16),
        child: Row(
          mainAxisAlignment: MainAxisAlignment.center,
          crossAxisAlignment: CrossAxisAlignment.center,
          children: [
            _book?.filePath != null
                ? Container(
                    constraints: const BoxConstraints(
                      maxWidth: 600,
                      maxHeight: 600,
                    ),
                    child: Image.network(
                      "${Globals.apiAddress}/images/books/${_book?.filePath}.webp",
                      fit: BoxFit.cover,
                      errorBuilder: (context, error, stackTrace) =>
                          const Icon(Icons.broken_image, size: 400),
                    ),
                  )
                : const SizedBox(
                    width: 400,
                    height: 400,
                    child: Icon(Icons.image_not_supported, size: 400),
                  ),
            const SizedBox(width: 70),
            SizedBox(
              width: 600,
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    _book?.title ?? "",
                    style: const TextStyle(
                      fontSize: 40,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  const SizedBox(height: 2),
                  Text(
                    "${_book?.price?.toStringAsFixed(2)}€",
                    style: const TextStyle(
                      fontSize: 22,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  const SizedBox(height: 20),
                  Text(
                    _book?.description ?? "Description is not set",
                    style: const TextStyle(fontSize: 16),
                  ),
                  const SizedBox(height: 30),
                  RichText(
                    text: TextSpan(
                      style: const TextStyle(
                        fontSize: 16,
                        fontWeight: FontWeight.w500,
                      ),
                      children: [
                        const TextSpan(
                          text: "Publisher: ",
                          style: TextStyle(color: Colors.black),
                        ),
                        TextSpan(
                          text: _book?.publisher?.userName ?? "",
                          style: TextStyle(color: Globals.backgroundColor),
                          recognizer: TapGestureRecognizer()
                            ..onTap = () {
                              Navigator.pop(context);
                              Navigator.pushReplacement(
                                context,
                                MaterialPageRoute(
                                  builder: (context) => UsersScreen(
                                    userName: _book?.publisher?.userName,
                                  ),
                                ),
                              );
                            },
                        ),
                      ],
                    ),
                  ),
                  const SizedBox(height: 2),
                  RichText(
                    text: TextSpan(
                      style: const TextStyle(
                        fontSize: 16,
                        fontWeight: FontWeight.w500,
                        color: Colors.black,
                      ),
                      children: [
                        TextSpan(
                          text: (_book?.authors?.length ?? 1) == 1
                              ? "Author: "
                              : "Authors: ",
                        ),
                        ...?_book?.authors?.asMap().entries.expand((entry) {
                          final index = entry.key;
                          final author = entry.value;
                          final isLast = index == _book!.authors!.length - 1;
                          return [
                            TextSpan(
                              text: author.name ?? "",
                              style: TextStyle(color: Globals.backgroundColor),
                              recognizer: TapGestureRecognizer()
                                ..onTap = () {
                                  Navigator.pop(context);
                                  Navigator.pushReplacement(
                                    context,
                                    MaterialPageRoute(
                                      builder: (context) =>
                                          AuthorsScreen(name: author.name),
                                    ),
                                  );
                                },
                            ),
                            if (!isLast)
                              const TextSpan(
                                text: ", ",
                                style: TextStyle(color: Colors.black),
                              ),
                          ];
                        }),
                      ],
                    ),
                  ),
                  const SizedBox(height: 2),
                  RichText(
                    text: TextSpan(
                      style: const TextStyle(
                        fontSize: 16,
                        fontWeight: FontWeight.w500,
                        color: Colors.black,
                      ),
                      children: [
                        TextSpan(
                          text: (_book?.genres?.length ?? 1) == 1
                              ? "Genre: "
                              : "Genres: ",
                        ),
                        ...?_book?.genres?.asMap().entries.expand((entry) {
                          final index = entry.key;
                          final genre = entry.value;
                          final isLast = index == _book!.genres!.length - 1;
                          return [
                            TextSpan(
                              text: genre.name ?? "",
                              style: TextStyle(color: Globals.backgroundColor),
                              recognizer: TapGestureRecognizer()
                                ..onTap = () {
                                  Navigator.pop(context);
                                  Navigator.pushReplacement(
                                    context,
                                    MaterialPageRoute(
                                      builder: (context) =>
                                          GenresScreen(name: genre.name),
                                    ),
                                  );
                                },
                            ),
                            if (!isLast)
                              const TextSpan(
                                text: ", ",
                                style: TextStyle(color: Colors.black),
                              ),
                          ];
                        }),
                      ],
                    ),
                  ),
                  const SizedBox(height: 2),
                  RichText(
                    text: TextSpan(
                      style: const TextStyle(
                        fontSize: 16,
                        fontWeight: FontWeight.w500,
                      ),
                      children: [
                        const TextSpan(
                          text: "Language: ",
                          style: TextStyle(color: Colors.black),
                        ),
                        TextSpan(
                          text: _book?.language?.name ?? "",
                          style: TextStyle(color: Globals.backgroundColor),
                          recognizer: TapGestureRecognizer()
                            ..onTap = () {
                              Navigator.pop(context);
                              Navigator.pushReplacement(
                                context,
                                MaterialPageRoute(
                                  builder: (context) => LanguagesScreen(
                                    name: _book?.language?.name,
                                  ),
                                ),
                              );
                            },
                        ),
                      ],
                    ),
                  ),
                  const SizedBox(height: 2),
                  RichText(
                    text: TextSpan(
                      style: const TextStyle(
                        fontSize: 16,
                        fontWeight: FontWeight.w500,
                      ),
                      children: [
                        const TextSpan(
                          text: "Number of pages: ",
                          style: TextStyle(color: Colors.black),
                        ),
                        TextSpan(
                          text: _book?.numberOfPages.toString(),
                          style: TextStyle(color: Colors.black),
                        ),
                      ],
                    ),
                  ),
                  const SizedBox(height: 2),
                  RichText(
                    text: TextSpan(
                      style: const TextStyle(
                        fontSize: 16,
                        fontWeight: FontWeight.w500,
                      ),
                      children: [
                        const TextSpan(
                          text: "Status: ",
                          style: TextStyle(color: Colors.black),
                        ),
                        TextSpan(
                          text: _book?.status,
                          style: TextStyle(color: Globals.backgroundColor),
                          recognizer: TapGestureRecognizer()
                            ..onTap = () {
                              Navigator.pop(context);
                              Navigator.pushReplacement(
                                context,
                                MaterialPageRoute(
                                  builder: (context) =>
                                      BooksScreen(status: _book?.status),
                                ),
                              );
                            },
                        ),
                      ],
                    ),
                  ),
                  const SizedBox(height: 24),
                  Column(
                    children: [
                      SizedBox(
                        width: 300,
                        child: ElevatedButton(
                          onPressed: () async {
                            await _downloadBookFile(widget.bookId);
                          },
                          style: ElevatedButton.styleFrom(
                            backgroundColor: Globals.backgroundColor,
                            shape: const RoundedRectangleBorder(
                              borderRadius: BorderRadius.all(
                                Radius.circular(10),
                              ),
                            ),
                            padding: const EdgeInsets.symmetric(vertical: 16),
                          ),
                          child: const Text(
                            "Book",
                            style: TextStyle(
                              fontSize: 16,
                              color: Globals.color,
                            ),
                          ),
                        ),
                      ),
                      if (_downloadProgress != null)
                        SizedBox(
                          width: 300,
                          child: Column(
                            children: [
                              const SizedBox(height: 8),
                              LinearProgressIndicator(
                                value: _downloadProgress,
                                minHeight: 8,
                                backgroundColor: Colors.grey[300],
                                valueColor: AlwaysStoppedAnimation<Color>(
                                  Globals.backgroundColor,
                                ),
                              ),
                            ],
                          ),
                        ),
                      const SizedBox(height: 12),
                      SizedBox(
                        width: 300,
                        child: OutlinedButton(
                          onPressed: () async {
                            await _openUrl(
                              "${Globals.apiAddress}/pdfs/summary/${_book?.filePath}.pdf",
                            );
                          },
                          style: OutlinedButton.styleFrom(
                            side: const BorderSide(
                              color: Globals.backgroundColor,
                              width: 2,
                            ),
                            shape: const RoundedRectangleBorder(
                              borderRadius: BorderRadius.all(
                                Radius.circular(10),
                              ),
                            ),
                            padding: const EdgeInsets.symmetric(vertical: 16),
                          ),
                          child: const Text(
                            "Summary",
                            style: TextStyle(
                              fontSize: 16,
                              color: Globals.backgroundColor,
                            ),
                          ),
                        ),
                      ),
                    ],
                  ),
                  if (_book?.deletionReason != null) ...[
                    const SizedBox(height: 16),
                    const Text(
                      "This book is deleted",
                      style: TextStyle(fontWeight: FontWeight.bold),
                    ),
                    Text(_book?.deletionReason ?? ""),
                  ],
                  const SizedBox(height: 32),
                  Row(
                    children: [
                      if (_allowedActions.contains("Approve")) ...[
                        SizedBox(
                          width: 140,
                          child: ElevatedButton(
                            onPressed: () async {
                              await _approveBookDialog();
                            },
                            style: ElevatedButton.styleFrom(
                              backgroundColor: Globals.backgroundColor,
                              shape: const RoundedRectangleBorder(
                                borderRadius: BorderRadius.all(
                                  Radius.circular(10),
                                ),
                              ),
                              padding: const EdgeInsets.symmetric(vertical: 16),
                            ),
                            child: const Text(
                              "Approve",
                              style: TextStyle(
                                fontSize: 16,
                                color: Globals.color,
                              ),
                            ),
                          ),
                        ),
                        const SizedBox(width: 20),
                      ],
                      if (_allowedActions.contains("Reject")) ...[
                        SizedBox(
                          width: 140,
                          child: ElevatedButton(
                            onPressed: () async {
                              await _rejectBookDialog();
                            },
                            style: ElevatedButton.styleFrom(
                              backgroundColor: Globals.backgroundColor,
                              shape: const RoundedRectangleBorder(
                                borderRadius: BorderRadius.all(
                                  Radius.circular(10),
                                ),
                              ),
                              padding: const EdgeInsets.symmetric(vertical: 16),
                            ),
                            child: const Text(
                              "Reject",
                              style: TextStyle(
                                fontSize: 16,
                                color: Globals.color,
                              ),
                            ),
                          ),
                        ),
                        const SizedBox(width: 20),
                      ],
                      if (AuthProvider.role == "Admin")
                        SizedBox(
                          width: 140,
                          child: ElevatedButton(
                            onPressed: () async {
                              if (_book?.deletionReason == null) {
                                await _deleteBookDialog(context);
                              } else {
                                await _undoDeleteBookDialog(context);
                              }
                            },
                            style: ElevatedButton.styleFrom(
                              backgroundColor: Colors.red,
                              shape: const RoundedRectangleBorder(
                                borderRadius: BorderRadius.all(
                                  Radius.circular(10),
                                ),
                              ),
                              padding: const EdgeInsets.symmetric(vertical: 16),
                            ),
                            child: Text(
                              _book?.deletionReason == null
                                  ? "Delete"
                                  : "Undo delete",
                              style: TextStyle(
                                fontSize: 16,
                                color: Globals.color,
                              ),
                            ),
                          ),
                        ),
                    ],
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}
