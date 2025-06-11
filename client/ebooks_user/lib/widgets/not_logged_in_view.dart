import "package:ebooks_user/utils/globals.dart";
import "package:flutter/material.dart";
import "package:ebooks_user/screens/profile_screen.dart";

class NotLoggedInView extends StatelessWidget {
  const NotLoggedInView({super.key});

  @override
  Widget build(BuildContext context) {
    return Column(
      mainAxisSize: MainAxisSize.min,
      children: [
        const Text("You are not logged in", style: TextStyle(fontSize: 18)),
        const SizedBox(height: 12),
        ElevatedButton(
          onPressed: () {
            Globals.pageIndex = 3;
            Navigator.push(
              context,
              MaterialPageRoute(builder: (_) => const ProfileScreen()),
            );
          },
          child: const Text("Log in"),
        ),
      ],
    );
  }
}
