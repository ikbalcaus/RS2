import "dart:convert";
import "package:ebooks_admin/models/questions/question.dart";
import "package:ebooks_admin/providers/base_provider.dart";
import "package:ebooks_admin/utils/globals.dart";
import "package:http/http.dart" as http;

class QuestionsProvider extends BaseProvider<Question> {
  QuestionsProvider() : super("questions");

  @override
  Question fromJson(data) {
    return Question.fromJson(data);
  }

  Future patch(int id, dynamic request) async {
    var uri = Uri.parse("${Globals.apiAddress}/questions/$id");
    var headers = createHeaders();
    var jsonRequest = jsonEncode(request);
    var response = await http.patch(uri, headers: headers, body: jsonRequest);
    if (!isValidResponse(response)) {
      throw response.body;
    }
  }
}
