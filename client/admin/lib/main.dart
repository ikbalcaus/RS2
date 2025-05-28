import 'package:flutter/material.dart';
import '../screens/login_screen.dart';

void main() {
  runApp(MaterialApp(
    title: "eBooks Dashboard",
    theme: ThemeData(
      colorScheme: ColorScheme.fromSeed(seedColor: Colors.blue),
    ),
    home: LoginScreen(),
  ));
}
