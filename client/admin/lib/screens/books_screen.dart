import "package:flutter/material.dart";
import "package:admin/screens/master_screen.dart";

class BooksScreen extends StatelessWidget {
  const BooksScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return MasterScreen(child: const Text("Books screen"));
  }
}
