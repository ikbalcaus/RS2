import "dart:convert";
import "dart:io";
import "package:ebooks_admin/models/books/book.dart";
import "package:ebooks_admin/providers/base_provider.dart";
import "package:ebooks_admin/utils/globals.dart";
import "package:http/http.dart" as http;

class BooksProvider extends BaseProvider<Book> {
  BooksProvider() : super("books");

  @override
  Book fromJson(data) {
    return Book.fromJson(data);
  }

  Future adminDelete(int id, String? reason) async {
    var uri = Uri.parse("${Globals.apiAddress}/books/$id/admin-delete");
    if (reason != null && reason.isNotEmpty) {
      uri = uri.replace(queryParameters: {"reason": reason});
    }
    var headers = createHeaders();
    var response = await http.delete(uri, headers: headers);
    if (!isValidResponse(response)) {
      throw response.body;
    }
  }

  Future getBookStates() async {
    try {
      var uri = Uri.parse("${Globals.apiAddress}/books/book-states");
      var headers = createHeaders();
      var response = await http.get(uri, headers: headers);
      if (isValidResponse(response)) {
        var data = jsonDecode(response.body);
        return (data as List<dynamic>).cast<String>();
      } else {
        throw response.body;
      }
    } on SocketException {
    } catch (ex) {}
  }

  Future getAllowedActions(int id) async {
    try {
      var uri = Uri.parse(
        "${Globals.apiAddress}/books/$id/admin-allowed-actions",
      );
      var headers = createHeaders();
      var response = await http.get(uri, headers: headers);
      if (isValidResponse(response)) {
        var data = jsonDecode(response.body);
        return (data as List<dynamic>).cast<String>();
      } else {
        throw response.body;
      }
    } on SocketException {
    } catch (ex) {}
  }

  Future approveBook(int id) async {
    var uri = Uri.parse("${Globals.apiAddress}/books/$id/approve");
    var headers = createHeaders();
    var response = await http.patch(uri, headers: headers);
    if (!isValidResponse(response)) {
      throw response.body;
    }
  }

  Future rejectBook(int id, String reason) async {
    var uri = Uri.parse("${Globals.apiAddress}/books/$id/reject");
    if (reason.isNotEmpty) {
      uri = uri.replace(queryParameters: {"reason": reason});
    }
    var headers = createHeaders();
    var response = await http.patch(uri, headers: headers);
    if (!isValidResponse(response)) {
      throw response.body;
    }
  }
}
