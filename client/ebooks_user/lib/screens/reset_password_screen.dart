import "package:ebooks_user/providers/users_provider.dart";
import "package:ebooks_user/utils/globals.dart";
import "package:ebooks_user/utils/helpers.dart";
import "package:flutter/material.dart";
import "package:provider/provider.dart";
import "package:easy_localization/easy_localization.dart";

class ResetPasswordScreen extends StatefulWidget {
  final String token;

  const ResetPasswordScreen({super.key, required this.token});

  @override
  State<ResetPasswordScreen> createState() => _ResetPasswordScreenState();
}

class _ResetPasswordScreenState extends State<ResetPasswordScreen> {
  late UsersProvider _usersProvider;
  final TextEditingController _passwordController = TextEditingController();
  final TextEditingController _confirmPasswordController =
      TextEditingController();
  final _formKey = GlobalKey<FormState>();

  @override
  void initState() {
    super.initState();
    _usersProvider = context.read<UsersProvider>();
  }

  @override
  void dispose() {
    _passwordController.dispose();
    _confirmPasswordController.dispose();
    super.dispose();
  }

  Future _resetPassword() async {
    if (!_formKey.currentState!.validate()) return;
    try {
      await _usersProvider.resetPassword(
        widget.token,
        _passwordController.text,
      );
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text("Lozinka je uspješno resetovana".tr())),
        );
        Navigator.pop(context);
      }
    } catch (ex) {
      if (!mounted) return;
      Helpers.showErrorMessage(context, ex);
    }
  }

  @override
  Widget build(BuildContext context) {
    final isDarkMode = Theme.of(context).brightness == Brightness.dark;
    return Scaffold(
      appBar: AppBar(title: Text("Resetuj lozinku".tr())),
      body: Center(
        child: SingleChildScrollView(
          padding: const EdgeInsets.symmetric(horizontal: 24),
          child: Container(
            padding: const EdgeInsets.all(32),
            width: 400,
            decoration: BoxDecoration(
              color: isDarkMode ? const Color(0xFF181620) : Colors.white,
              borderRadius: BorderRadius.circular(4),
              boxShadow: [
                BoxShadow(
                  color: Colors.black.withOpacity(0.05),
                  blurRadius: 12,
                  offset: const Offset(0, 4),
                ),
              ],
            ),
            child: Form(
              key: _formKey,
              child: Column(
                children: [
                  Text(
                    "Resetuj lozinku".tr(),
                    style: const TextStyle(
                      fontSize: 28,
                      fontWeight: FontWeight.w700,
                    ),
                  ),
                  const SizedBox(height: 24),
                  TextFormField(
                    controller: _passwordController,
                    decoration: InputDecoration(
                      labelText: "Nova lozinka".tr(),
                      prefixIcon: const Icon(Icons.lock_outline),
                    ),
                    obscureText: true,
                    validator: (value) {
                      if (value == null || value.isEmpty) {
                        return "Ovo polje je obavezno".tr();
                      }
                      return null;
                    },
                  ),
                  const SizedBox(height: 10),
                  TextFormField(
                    controller: _confirmPasswordController,
                    decoration: InputDecoration(
                      labelText: "Potvrdi lozinku".tr(),
                      prefixIcon: const Icon(Icons.lock_outline),
                    ),
                    obscureText: true,
                    validator: (value) {
                      if (value == null || value.isEmpty) {
                        return "Ovo polje je obavezno".tr();
                      }
                      if (value != _passwordController.text) {
                        return "Lozinke se ne podudaraju".tr();
                      }
                      return null;
                    },
                  ),
                  const SizedBox(height: 24),
                  SizedBox(
                    width: double.infinity,
                    child: ElevatedButton(
                      onPressed: () async => await _resetPassword(),
                      style: ElevatedButton.styleFrom(
                        backgroundColor: Globals.backgroundColor,
                        padding: const EdgeInsets.symmetric(vertical: 16),
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(2),
                        ),
                      ),
                      child: Text(
                        "Potvrdi".tr(),
                        style: const TextStyle(
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
