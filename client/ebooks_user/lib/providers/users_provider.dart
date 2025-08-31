import "dart:convert";
import "dart:io";
import "package:ebooks_user/models/users/user.dart";
import "package:ebooks_user/providers/base_provider.dart";
import "package:ebooks_user/utils/globals.dart";
import "package:http/http.dart" as http;
import "package:http_parser/http_parser.dart";
import "package:mime/mime.dart";

class UsersProvider extends BaseProvider<User> {
  UsersProvider() : super("users");

  @override
  User fromJson(data) {
    return User.fromJson(data);
  }

  Future editUser(int id, dynamic request) async {
    var uri = Uri.parse("${Globals.apiAddress}/users/$id");
    var multipartRequest = http.MultipartRequest("PUT", uri);
    multipartRequest.headers.addAll(createHeaders(multipart: true));
    if (request is Map<String, dynamic>) {
      for (var entry in request.entries) {
        var key = entry.key;
        var value = entry.value;
        if (key == "imageFile" && value != null && value is File) {
          final mimeType = lookupMimeType(value.path);
          if (mimeType != null) {
            final multipartFile = await http.MultipartFile.fromPath(
              "imageFile",
              value.path,
              contentType: MediaType.parse(mimeType),
            );
            multipartRequest.files.add(multipartFile);
          }
        } else if (value != null) {
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

  Future forgotPassword(String email) async {
    try {
      var uri = Uri.parse(
        "${Globals.apiAddress}/users/forgot-password?email=$email",
      );
      var headers = createHeaders();
      var response = await http.patch(uri, headers: headers);
      if (!isValidResponse(response)) {
        throw response.body;
      }
    } on SocketException {
      throw "No internet connection";
    } catch (ex) {
      if (ex.toString().contains("Not found")) {
        throw "Email not found";
      } else {
        throw ex.toString();
      }
    }
  }

  Future resetPassword(String token, String password) async {
    try {
      var uri = Uri.parse(
        "${Globals.apiAddress}/users/reset-password?token=$token&password=$password",
      );
      var headers = createHeaders();
      var response = await http.patch(uri, headers: headers);
      if (!isValidResponse(response)) {
        throw response.body;
      }
    } on SocketException {
      throw "No internet connection";
    } catch (ex) {
      if (ex.toString().contains("Not found")) {
        throw "Email not found";
      } else {
        throw ex.toString();
      }
    }
  }

  Future verifyEmail(int id, String? token) async {
    var uri = Uri.parse(
      "${Globals.apiAddress}/users/$id/verify-email/${token ?? ""}",
    );
    var headers = createHeaders();
    var response = await http.patch(uri, headers: headers);
    if (!isValidResponse(response)) {
      throw response.body;
    }
  }
}
