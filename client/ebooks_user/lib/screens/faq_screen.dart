import "package:ebooks_user/providers/questions_provider.dart";
import "package:ebooks_user/screens/master_screen.dart";
import "package:ebooks_user/utils/globals.dart";
import "package:ebooks_user/utils/helpers.dart";
import "package:flutter/material.dart";
import "package:provider/provider.dart";

class FaqScreen extends StatefulWidget {
  const FaqScreen({super.key});

  @override
  State<FaqScreen> createState() => _FaqScreenState();
}

class _FaqScreenState extends State<FaqScreen> {
  late QuestionsProvider _questionsProvider;
  final TextEditingController _questionController = TextEditingController();

  final List<Map<String, String>> _faqList = [
    {
      "question": "How do I purchase a book?",
      "answer":
          "Select a book and click the \"Buy\" button. You can pay using your Credit Card or Paypal.",
    },
    {
      "question": "Can I read books offline?",
      "answer":
          "Yes, once purchased, books are available offline in your library.",
    },
  ];

  @override
  void initState() {
    super.initState();
    _questionsProvider = context.read<QuestionsProvider>();
  }

  @override
  void dispose() {
    _questionController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return MasterScreen(showBackButton: true, child: _buildResultView());
  }

  Future _submitQuestion() async {
    if (_questionController.text.trim().isNotEmpty) {
      try {
        await _questionsProvider.post({
          "message": _questionController.text,
        }, null);
        Helpers.showSuccessMessage(context, "Question has been sent");
        Navigator.pop(context);
      } catch (ex) {
        Helpers.showErrorMessage(context, ex);
      }
    }
  }

  Widget _buildResultView() {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 12, horizontal: 6),
      child: Column(
        children: [
          const Text(
            "Frequently asked questions",
            style: TextStyle(fontSize: 14, fontWeight: FontWeight.w500),
          ),
          SizedBox(height: 10),
          Expanded(
            child: ListView.builder(
              itemCount: _faqList.length,
              itemBuilder: (context, index) {
                return ExpansionTile(
                  title: Text(_faqList[index]["question"] ?? ""),
                  children: [
                    Padding(
                      padding: const EdgeInsets.all(8),
                      child: Text(_faqList[index]["answer"] ?? ""),
                    ),
                  ],
                );
              },
            ),
          ),
          const Divider(),
          const SizedBox(height: 8),
          Row(
            children: [
              SizedBox(width: 5),
              Expanded(
                child: TextField(
                  controller: _questionController,
                  decoration: const InputDecoration(
                    labelText: "Your question",
                    contentPadding: EdgeInsets.symmetric(horizontal: 6),
                  ),
                ),
              ),
              const SizedBox(width: 10),
              IconButton(
                onPressed: () async {
                  await _submitQuestion();
                },
                icon: const Icon(Icons.send),
                tooltip: "Send",
                color: Globals.backgroundColor,
              ),
            ],
          ),
        ],
      ),
    );
  }
}
