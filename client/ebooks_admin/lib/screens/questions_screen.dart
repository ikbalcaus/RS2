import "package:ebooks_admin/models/questions/question.dart";
import "package:ebooks_admin/models/search_result.dart";
import "package:ebooks_admin/providers/questions_provider.dart";
import "package:ebooks_admin/screens/master_screen.dart";
import "package:ebooks_admin/utils/constants.dart";
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

  final TextEditingController _questionEditingController =
      TextEditingController();
  final TextEditingController _askedByEditingController =
      TextEditingController();

  @override
  void initState() {
    super.initState();
    _questionsProvider = context.read<QuestionsProvider>();
    fetchQuestions();
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

  Future fetchQuestions() async {
    setState(() {
      _isLoading = true;
    });
    try {
      final questions = await _questionsProvider.getPaged(
        page: _currentPage,
        filter: _currentFilter,
      );
      if (!mounted) {
        return;
      }
      setState(() {
        _questions = questions;
      });
    } catch (ex) {
      if (!mounted) return;
      WidgetsBinding.instance.addPostFrameCallback((_) {
        if (!mounted) {
          return;
        }
        Helpers.showErrorMessage(context, ex);
      });
    } finally {
      if (!mounted) {
        return;
      }
      setState(() {
        _isLoading = false;
      });
    }
  }

  Future _answerQuestionDialog(BuildContext context, int id) async {
    final TextEditingController answerController = TextEditingController();
    await showDialog(
      context: context,
      builder: (dialogContext) {
        return AlertDialog(
          title: const Text("Answer"),
          content: TextField(
            controller: answerController,
            decoration: const InputDecoration(labelText: "Enter an answer..."),
          ),
          actions: [
            TextButton(
              onPressed: () async {
                final dialogText = answerController.text.trim();
                if (dialogText.isNotEmpty) {
                  Navigator.of(dialogContext).pop(true);
                  await Future.delayed(const Duration(milliseconds: 250));
                  try {
                    await _questionsProvider.patch(id, {"message": dialogText});
                    Helpers.showSuccessMessage(context);
                    await fetchQuestions();
                  } catch (ex) {
                    Helpers.showErrorMessage(context, ex);
                  }
                }
              },
              child: const Text("Answer"),
            ),
            TextButton(
              onPressed: () async {
                Navigator.of(dialogContext).pop(false);
              },
              child: const Text("Cancel"),
            ),
          ],
        );
      },
    );
  }

  Widget _buildSearch() {
    return Padding(
      padding: const EdgeInsets.all(Constants.defaultSpacing),
      child: Row(
        children: [
          Expanded(
            child: TextField(
              controller: _questionEditingController,
              decoration: const InputDecoration(labelText: "Question"),
            ),
          ),
          const SizedBox(width: Constants.defaultSpacing),
          Expanded(
            child: TextField(
              controller: _askedByEditingController,
              decoration: const InputDecoration(labelText: "Asked by"),
            ),
          ),
          const SizedBox(width: Constants.defaultSpacing),
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
          const SizedBox(width: Constants.defaultSpacing),
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
          const SizedBox(width: Constants.defaultSpacing),
          ElevatedButton(
            onPressed: () async {
              _currentPage = 1;
              _currentFilter = {
                "Question": _questionEditingController.text,
                "AskedBy": _askedByEditingController.text,
                "Status": _status,
                "OrderBy": _orderBy,
              };
              await fetchQuestions();
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
                        DataCell(Text(question.question1 ?? "")),
                        DataCell(Text(question.user?.userName ?? "")),
                        DataCell(Text(question.answer ?? "")),
                        DataCell(Text(question.answeredBy?.userName ?? "")),
                        DataCell(
                          Row(
                            children: [
                              IconButton(
                                icon: const Icon(Icons.question_answer),
                                tooltip: "Answer question",
                                onPressed: () async {
                                  await _answerQuestionDialog(
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
      padding: const EdgeInsets.symmetric(vertical: Constants.defaultSpacing),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          IconButton(
            icon: const Icon(Icons.chevron_left),
            onPressed: _currentPage > 1
                ? () async {
                    _isLoading = true;
                    _currentPage -= 1;
                    await fetchQuestions();
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
                    await fetchQuestions();
                  }
                : null,
          ),
        ],
      ),
    );
  }
}
