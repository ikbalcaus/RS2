import "package:ebooks_user/providers/users_provider.dart";
import "package:ebooks_user/screens/master_screen.dart";
import "package:ebooks_user/utils/helpers.dart";
import "package:flutter/material.dart";
import "package:provider/provider.dart";
import "package:ebooks_user/utils/globals.dart";

class ForgotPasswordScreen extends StatefulWidget {
  const ForgotPasswordScreen({super.key});

  @override
  State<ForgotPasswordScreen> createState() => _ForgotPasswordScreenState();
}

class _ForgotPasswordScreenState extends State<ForgotPasswordScreen> {
  late UsersProvider _usersProvider;
  final TextEditingController _emailController = TextEditingController();
  final _formKey = GlobalKey<FormState>();

  @override
  void initState() {
    super.initState();
    _usersProvider = context.read<UsersProvider>();
  }

  @override
  void dispose() {
    _emailController.dispose();
    super.dispose();
  }

  Future _forgotPassword() async {
    try {
      await _usersProvider.forgotPassword(_emailController.text);
      if (mounted) {
        Navigator.pop(context);
        Helpers.showSuccessMessage(
          context,
          "Verification link has been sent to you email",
        );
      }
    } catch (ex) {
      if (!mounted) return;
      Helpers.showErrorMessage(context, ex);
    }
  }

  @override
  Widget build(BuildContext context) {
    final isDarkMode = Theme.of(context).brightness == Brightness.dark;
    return MasterScreen(
      showBackButton: true,
      child: Center(
        child: SingleChildScrollView(
          padding: const EdgeInsets.symmetric(horizontal: 24),
          child: Container(
            padding: const EdgeInsets.all(32),
            decoration: BoxDecoration(
              color: isDarkMode
                  ? const Color.fromARGB(255, 24, 22, 32)
                  : Colors.white,
              borderRadius: BorderRadius.circular(4),
              boxShadow: [
                BoxShadow(
                  color: Colors.black.withOpacity(0.05),
                  blurRadius: 12,
                  offset: const Offset(0, 4),
                ),
              ],
            ),
            width: 400,
            child: Form(
              key: _formKey,
              child: Column(
                children: [
                  Text(
                    "Forgot password",
                    style: TextStyle(
                      fontSize: 28,
                      fontWeight: FontWeight.w700,
                      color: isDarkMode ? Colors.white : Colors.black,
                    ),
                  ),
                  const SizedBox(height: 24),
                  TextFormField(
                    controller: _emailController,
                    decoration: InputDecoration(
                      prefixIcon: const Icon(Icons.email_outlined),
                      labelText: "Email",
                    ),
                    keyboardType: TextInputType.emailAddress,
                    validator: (value) {
                      if (value == null || value.isEmpty) {
                        return "This field is required";
                      }
                      return null;
                    },
                  ),
                  const SizedBox(height: 24),
                  SizedBox(
                    width: double.infinity,
                    child: ElevatedButton(
                      onPressed: () async => await _forgotPassword(),
                      style: ElevatedButton.styleFrom(
                        backgroundColor: Globals.backgroundColor,
                        padding: const EdgeInsets.symmetric(vertical: 16),
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(2),
                        ),
                      ),
                      child: const Text(
                        "Submit",
                        style: TextStyle(
                          fontSize: 16,
                          color: Colors.white,
                          fontWeight: FontWeight.w500,
                        ),
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}
