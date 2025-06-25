import "dart:convert";
import "dart:io";
import "package:ebooks_user/models/books/book.dart";
import "package:ebooks_user/providers/base_provider.dart";
import "package:ebooks_user/utils/globals.dart";
import "package:http/http.dart" as http;
import "package:http_parser/http_parser.dart";
import "package:mime/mime.dart";

class BooksProvider extends BaseProvider<Book> {
  BooksProvider() : super("books");

  @override
  Book fromJson(data) {
    return Book.fromJson(data);
  }

  Future editBook(int? id, dynamic request) async {
    var uri = Uri.parse("${Globals.apiAddress}/books/${id ?? ""}");
    var multipartRequest = http.MultipartRequest(
      id == null ? "POST" : "PUT",
      uri,
    );
    multipartRequest.headers.addAll(createHeaders(multipart: true));
    if (request is Map<String, dynamic>) {
      for (var entry in request.entries) {
        var key = entry.key;
        var value = entry.value;
        if (value == null) continue;
        if ((key == "imageFile" ||
                key == "bookPdfFile" ||
                key == "summaryPdfFile") &&
            value is File) {
          final mimeType = lookupMimeType(value.path);
          if (mimeType != null) {
            final multipartFile = await http.MultipartFile.fromPath(
              key,
              value.path,
              contentType: MediaType.parse(mimeType),
            );
            multipartRequest.files.add(multipartFile);
          }
        } else {
          multipartRequest.fields[key] = value.toString();
        }
      }
    }
    var streamedResponse = await multipartRequest.send();
    var responseBody = await streamedResponse.stream.bytesToString();
    if (streamedResponse.statusCode < 300) {
      var data = jsonDecode(responseBody);
      return fromJson(data);
    } else {
      throw responseBody;
    }
  }

  Future getBookStates() async {
    var uri = Uri.parse("${Globals.apiAddress}/books/book-states");
    var headers = createHeaders();
    var response = await http.get(uri, headers: headers);
    if (isValidResponse(response)) {
      var data = jsonDecode(response.body);
      return (data as List<dynamic>).cast<String>();
    } else {
      throw response.body;
    }
  }

  Future getAllowedActions(int id) async {
    var uri = Uri.parse("${Globals.apiAddress}/books/$id/user-allowed-actions");
    var headers = createHeaders();
    var response = await http.get(uri, headers: headers);
    if (isValidResponse(response)) {
      var data = jsonDecode(response.body);
      return (data as List<dynamic>).cast<String>();
    } else {
      throw response.body;
    }
  }

  Future awaitBook(int id) async {
    var uri = Uri.parse("${Globals.apiAddress}/books/$id/await");
    var headers = createHeaders();
    var response = await http.patch(uri, headers: headers);
    if (!isValidResponse(response)) {
      throw response.body;
    }
  }

  Future hideBook(int id) async {
    var uri = Uri.parse("${Globals.apiAddress}/books/$id/hide");
    var headers = createHeaders();
    var response = await http.patch(uri, headers: headers);
    if (!isValidResponse(response)) {
      throw response.body;
    }
  }
}
