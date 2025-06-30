import "package:ebooks_admin/models/questions/question.dart";
import "package:ebooks_admin/models/search_result.dart";
import "package:ebooks_admin/providers/questions_provider.dart";
import "package:ebooks_admin/screens/master_screen.dart";
import "package:ebooks_admin/utils/globals.dart";
import "package:ebooks_admin/utils/helpers.dart";
import "package:flutter/material.dart";
import "package:provider/provider.dart";

class QuestionsScreen extends StatefulWidget {
  const QuestionsScreen({super.key});

  @override
  State<QuestionsScreen> createState() => _QuestionsScreenState();
}

class _QuestionsScreenState extends State<QuestionsScreen> {
  late QuestionsProvider _questionsProvider;
  SearchResult<Question>? _questions;
  bool _isLoading = true;
  int _currentPage = 1;
  String _status = "All questions";
  String _orderBy = "Last added";
  Map<String, dynamic> _currentFilter = {};

  final TextEditingController _questionController = TextEditingController();
  final TextEditingController _askedByController = TextEditingController();

  @override
  void initState() {
    super.initState();
    _questionsProvider = context.read<QuestionsProvider>();
    _fetchQuestions();
  }

  @override
  void dispose() {
    _questionController.dispose();
    _askedByController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return MasterScreen(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          _buildSearch(),
          Expanded(
            child: _isLoading
                ? const Center(child: CircularProgressIndicator())
                : _buildResultView(),
          ),
          _buildPagination(),
        ],
      ),
    );
  }

  Future _fetchQuestions() async {
    setState(() => _isLoading = true);
    try {
      final questions = await _questionsProvider.getPaged(
        page: _currentPage,
        filter: _currentFilter,
      );
      if (!mounted) return;
      setState(() => _questions = questions);
    } catch (ex) {
      if (!mounted) return;
      WidgetsBinding.instance.addPostFrameCallback((_) {
        if (!mounted) return;
        Helpers.showErrorMessage(context, ex);
      });
    } finally {
      if (!mounted) return;
      setState(() => _isLoading = false);
    }
  }

  Future _showAnswerQuestionDialog(BuildContext context, int id) async {
    String answer = "";
    await showDialog(
      context: context,
      builder: (dialogContext) {
        return AlertDialog(
          title: const Text("Answer"),
          content: TextField(
            decoration: const InputDecoration(labelText: "Enter an answer..."),
            onChanged: (value) => answer = value,
          ),
          actions: [
            TextButton(
              onPressed: () async {
                if (answer.trim().isNotEmpty) {
                  try {
                    await _questionsProvider.patch(id, {"message": answer});
                    await _fetchQuestions();
                    if (context.mounted) {
                      Navigator.pop(context);
                      Helpers.showSuccessMessage(context);
                    }
                  } catch (ex) {
                    Helpers.showErrorMessage(context, ex);
                  }
                }
              },
              child: const Text("Answer"),
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

  Widget _buildSearch() {
    return Padding(
      padding: const EdgeInsets.all(Globals.spacing),
      child: Row(
        children: [
          Expanded(
            child: TextField(
              controller: _questionController,
              decoration: const InputDecoration(labelText: "Question"),
            ),
          ),
          const SizedBox(width: Globals.spacing),
          Expanded(
            child: TextField(
              controller: _askedByController,
              decoration: const InputDecoration(labelText: "Asked by"),
            ),
          ),
          const SizedBox(width: Globals.spacing),
          Expanded(
            child: DropdownButtonFormField<String>(
              value: _status,
              onChanged: (value) {
                _status = value!;
              },
              items: ["All questions", "Only answered", "Only not answered"]
                  .map((String value) {
                    return DropdownMenuItem<String>(
                      value: value,
                      child: Text(
                        value,
                        style: const TextStyle(fontWeight: FontWeight.normal),
                      ),
                    );
                  })
                  .toList(),
              decoration: const InputDecoration(labelText: "Status"),
            ),
          ),
          const SizedBox(width: Globals.spacing),
          Expanded(
            child: DropdownButtonFormField<String>(
              value: _orderBy,
              onChanged: (value) {
                _orderBy = value!;
              },
              items: ["Last added", "First added"].map((String value) {
                return DropdownMenuItem<String>(
                  value: value,
                  child: Text(
                    value,
                    style: const TextStyle(fontWeight: FontWeight.normal),
                  ),
                );
              }).toList(),
              decoration: const InputDecoration(labelText: "Sort by"),
            ),
          ),
          const SizedBox(width: Globals.spacing),
          ElevatedButton(
            onPressed: () async {
              _currentPage = 1;
              _currentFilter = {
                "Question": _questionController.text,
                "AskedBy": _askedByController.text,
                "Status": _status,
                "OrderBy": _orderBy,
              };
              await _fetchQuestions();
            },
            child: const Text("Search"),
          ),
        ],
      ),
    );
  }

  Widget _buildResultView() {
    return SizedBox(
      width: double.infinity,
      child: SingleChildScrollView(
        child: DataTable(
          columns: const [
            DataColumn(label: Text("Question")),
            DataColumn(label: Text("Asked by")),
            DataColumn(label: Text("Answer")),
            DataColumn(label: Text("Answered by")),
            DataColumn(label: Text("Actions")),
          ],
          rows:
              _questions?.resultList
                  .map(
                    (question) => DataRow(
                      cells: [
                        DataCell(
                          SizedBox(
                            width: 200,
                            child: Text(
                              question.question1 ?? "",
                              softWrap: true,
                            ),
                          ),
                        ),
                        DataCell(Text(question.user?.userName ?? "")),
                        DataCell(
                          SizedBox(
                            width: 200,
                            child: Text(question.answer ?? "", softWrap: true),
                          ),
                        ),
                        DataCell(Text(question.answeredBy?.userName ?? "")),
                        DataCell(
                          Row(
                            children: [
                              IconButton(
                                icon: const Icon(Icons.question_answer),
                                tooltip: "Answer question",
                                onPressed: () async {
                                  await _showAnswerQuestionDialog(
                                    context,
                                    question.questionId!,
                                  );
                                },
                              ),
                            ],
                          ),
                        ),
                      ],
                    ),
                  )
                  .toList() ??
              [],
        ),
      ),
    );
  }

  Widget _buildPagination() {
    if (_questions == null || _questions!.totalPages <= 1) {
      return const SizedBox.shrink();
    }
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: Globals.spacing),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          IconButton(
            icon: const Icon(Icons.chevron_left),
            onPressed: _currentPage > 1
                ? () async {
                    _isLoading = true;
                    _currentPage -= 1;
                    await _fetchQuestions();
                  }
                : null,
          ),
          Text("Page $_currentPage of ${_questions!.totalPages}"),
          IconButton(
            icon: const Icon(Icons.chevron_right),
            onPressed: _currentPage < _questions!.totalPages
                ? () async {
                    _isLoading = true;
                    _currentPage += 1;
                    await _fetchQuestions();
                  }
                : null,
          ),
        ],
      ),
    );
  }
}
