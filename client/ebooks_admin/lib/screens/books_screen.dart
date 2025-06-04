import "package:ebooks_admin/screens/master_screen.dart";
import "package:flutter/material.dart";

class BooksScreen extends StatefulWidget {
  const BooksScreen({super.key});

  @override
  State<BooksScreen> createState() => _BooksScreenState();
}

class _BooksScreenState extends State<BooksScreen> {
  bool _isLoading = true;

  @override
  Widget build(BuildContext context) {
    return MasterScreen(
      child: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : Text(""),
    );
  }
}
