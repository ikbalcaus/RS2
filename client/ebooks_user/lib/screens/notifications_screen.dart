import "package:ebooks_user/models/notifications/app_notification.dart";
import "package:ebooks_user/models/search_result.dart";
import "package:ebooks_user/providers/notifications_provider.dart";
import "package:ebooks_user/screens/book_details_screen.dart";
import "package:ebooks_user/screens/master_screen.dart";
import "package:ebooks_user/screens/publisher_screen.dart";
import "package:ebooks_user/utils/helpers.dart";
import "package:ebooks_user/widgets/not_logged_in_view.dart";
import "package:flutter/material.dart";
import "package:ebooks_user/providers/auth_provider.dart";
import "package:provider/provider.dart";

class NotificationsScreen extends StatefulWidget {
  const NotificationsScreen({super.key});

  @override
  State<NotificationsScreen> createState() => _NotificationsScreenState();
}

class _NotificationsScreenState extends State<NotificationsScreen> {
  late NotificationsProvider _notificationsProvider;
  SearchResult<AppNotification>? _notifications;
  int _currentPage = 1;
  bool _isLoading = true;
  final ScrollController _scrollController = ScrollController();

  @override
  void initState() {
    super.initState();
    if (AuthProvider.isLoggedIn) {
      _notificationsProvider = context.read<NotificationsProvider>();
      _fetchNotifications();
      _scrollController.addListener(_scrollListener);
    }
  }

  void _scrollListener() {
    if (!_scrollController.hasClients) return;
    if (_scrollController.position.pixels >=
        _scrollController.position.maxScrollExtent - 200) {
      if (!_isLoading &&
          (_notifications?.resultList.length ?? 0) <
              (_notifications?.count ?? 0)) {
        _currentPage++;
        _fetchNotifications(append: true);
      }
    }
  }

  @override
  void dispose() {
    _scrollController.removeListener(_scrollListener);
    _scrollController.dispose();
    super.dispose();
  }

  Future _fetchNotifications({bool append = false}) async {
    setState(() => _isLoading = true);
    try {
      final notifications = await _notificationsProvider.getPaged(
        page: _currentPage,
      );
      if (!mounted) return;
      setState(() {
        if (append && _notifications != null) {
          _notifications?.resultList.addAll(notifications.resultList);
          _notifications?.count = notifications.count;
        } else {
          _notifications = notifications;
        }
      });

      // automatski fetch dok lista ne popuni ekran
      WidgetsBinding.instance.addPostFrameCallback((_) {
        if (!mounted) return;
        if (!_scrollController.hasClients) return;

        final maxScroll = _scrollController.position.maxScrollExtent;
        if (!_isLoading &&
            (_notifications?.resultList.length ?? 0) <
                (_notifications?.count ?? 0) &&
            maxScroll <= 0) {
          _currentPage++;
          _fetchNotifications(append: true);
        }
      });
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

  @override
  Widget build(BuildContext context) {
    Widget content;
    if (!AuthProvider.isLoggedIn) {
      content = Center(child: NotLoggedInView());
    } else if (_isLoading && (_notifications?.resultList.isEmpty ?? true)) {
      content = const Center(child: CircularProgressIndicator());
    } else if (_notifications?.count == 0) {
      content = const Center(child: Text("You don't have any notifications"));
    } else {
      content = _buildResultView();
    }
    return MasterScreen(child: content);
  }

  Widget _buildResultView() {
    final items = _notifications?.resultList ?? [];
    return ListView.builder(
      controller: _scrollController,
      itemCount: items.length,
      itemBuilder: (context, index) {
        final notification = items[index];
        final isLast = index == items.length - 1;
        return Column(
          children: [
            ListTile(
              title: Text(notification.message ?? ""),
              trailing:
                  (notification.bookId != null ||
                      notification.publisherId != null)
                  ? const Icon(Icons.chevron_right_rounded)
                  : null,
              onTap: () async {
                await _notificationsProvider.markAsRead(
                  notification.notificationId!,
                );
                if (notification.bookId != null) {
                  Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (context) =>
                          BookDetailsScreen(bookId: notification.bookId!),
                    ),
                  );
                } else if (notification.publisherId != null) {
                  Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (context) => PublisherScreen(
                        publisherId: notification.publisherId!,
                      ),
                    ),
                  );
                }
              },
            ),
            const Divider(height: 1),
            if (isLast) const Divider(height: 0.1),
          ],
        );
      },
    );
  }
}
