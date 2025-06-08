import "dart:convert";
import "package:ebooks_admin/models/search_result.dart";
import "package:ebooks_admin/providers/auth_provider.dart";
import "package:ebooks_admin/utils/constants.dart";
import "package:flutter/material.dart";
import "package:http/http.dart" as http;

abstract class BaseProvider<T> with ChangeNotifier {
  String _endpoint = "";

  BaseProvider(String endpoint) {
    _endpoint = endpoint;
  }

  Future<SearchResult<T>> getPaged({
    int page = 1,
    int pageSize = 10,
    Map<String, dynamic>? filter,
  }) async {
    var url = "${Constants.apiAddress}/$_endpoint";
    final queryParams = <String, String>{};
    queryParams["Page"] = page.toString();
    queryParams["PageSize"] = pageSize.toString();
    if (filter != null) {
      filter.forEach((key, value) {
        if (value != null && value.toString().isNotEmpty) {
          queryParams[key] = value.toString();
        }
      });
    }
    var uri = Uri.parse(url).replace(queryParameters: queryParams);
    var headers = createHeaders();
    var response = await http.get(uri, headers: headers);
    if (isValidResponse(response)) {
      var data = jsonDecode(response.body);
      var result = SearchResult<T>();
      result.count = data["count"];
      for (var item in data["resultList"]) {
        result.resultList.add(fromJson(item));
      }
      result.totalPages = (result.count / pageSize).ceil();
      return result;
    } else {
      throw response.body;
    }
  }

  Future getById(int id) async {
    var uri = Uri.parse("${Constants.apiAddress}/$_endpoint/$id");
    var headers = createHeaders();
    var response = await http.get(uri, headers: headers);
    if (isValidResponse(response)) {
      var data = jsonDecode(response.body);
      return fromJson(data);
    } else {
      throw response.body;
    }
  }

  Future post(dynamic request) async {
    var uri = Uri.parse("${Constants.apiAddress}/$_endpoint");
    var headers = createHeaders();
    var jsonRequest = jsonEncode(request);
    var response = await http.post(uri, headers: headers, body: jsonRequest);
    if (isValidResponse(response)) {
      var data = jsonDecode(response.body);
      return fromJson(data);
    } else {
      throw response.body;
    }
  }

  Future put(int id, dynamic request) async {
    var uri = Uri.parse("${Constants.apiAddress}/$_endpoint/$id");
    var headers = createHeaders();
    var jsonRequest = jsonEncode(request);
    var response = await http.put(uri, headers: headers, body: jsonRequest);
    if (isValidResponse(response)) {
      var data = jsonDecode(response.body);
      return fromJson(data);
    } else {
      throw response.body;
    }
  }

  Future delete(int id) async {
    var uri = Uri.parse("${Constants.apiAddress}/$_endpoint/$id");
    var headers = createHeaders();
    var response = await http.delete(uri, headers: headers);
    if (!isValidResponse(response)) {
      throw response.body;
    }
  }

  T fromJson(data) {
    throw Exception("Error: Method not implemented");
  }

  bool isValidResponse(http.Response response) {
    if (response.statusCode < 299) {
      return true;
    }
    return false;
  }

  Map<String, String> createHeaders() {
    String username = AuthProvider.email ?? "";
    String password = AuthProvider.password ?? "";
    String basicAuth =
        "Basic ${base64Encode(utf8.encode("$username:$password"))}";
    final headers = {
      "Content-Type": "application/json",
      "Authorization": basicAuth,
    };
    return headers;
  }

  String getQueryString(
    Map params, {
    String prefix = "&",
    bool inRecursion = false,
  }) {
    String query = "";
    params.forEach((key, value) {
      if (inRecursion) {
        if (key is int) {
          key = "[$key]";
        } else if (value is List || value is Map) {
          key = ".$key";
        } else {
          key = ".$key";
        }
      }
      if (value is String || value is int || value is double || value is bool) {
        var encoded = value;
        if (value is String) {
          encoded = Uri.encodeComponent(value);
        }
        query += "$prefix$key=$encoded";
      } else if (value is DateTime) {
        query += "$prefix$key=${value.toIso8601String()}";
      } else if (value is List || value is Map) {
        if (value is List) value = value.asMap();
        value.forEach((k, v) {
          query += getQueryString(
            {k: v},
            prefix: "$prefix$key",
            inRecursion: true,
          );
        });
      }
    });
    return query;
  }
}
