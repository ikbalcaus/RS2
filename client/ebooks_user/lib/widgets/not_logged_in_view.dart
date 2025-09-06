import "package:ebooks_user/utils/globals.dart";
import "package:flutter/material.dart";
import "package:ebooks_user/screens/profile_screen.dart";
import "package:easy_localization/easy_localization.dart";

class NotLoggedInView extends StatelessWidget {
  const NotLoggedInView({super.key});

  @override
  Widget build(BuildContext context) {
    return Column(
      mainAxisSize: MainAxisSize.min,
      children: [
        Text("You are not logged in".tr(), style: TextStyle(fontSize: 18)),
        const SizedBox(height: 12),
        ElevatedButton(
          onPressed: () {
            Globals.pageIndex = 3;
            Navigator.push(
              context,
              MaterialPageRoute(builder: (_) => const ProfileScreen()),
            );
          },
          child: Text("Log in".tr()),
        ),
      ],
    );
  }
}
