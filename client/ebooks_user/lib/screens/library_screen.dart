import "package:ebooks_user/screens/master_screen.dart";
import "package:flutter/material.dart";

class LibraryScreen extends StatefulWidget {
  const LibraryScreen({super.key});

  @override
  State<LibraryScreen> createState() => _LibraryScreenState();
}

class _LibraryScreenState extends State<LibraryScreen> {
  @override
  Widget build(BuildContext context) {
    return MasterScreen(
      child: const Center(child: CircularProgressIndicator()),
    );
  }
}
