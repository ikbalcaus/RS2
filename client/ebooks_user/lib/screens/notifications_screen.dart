import "package:ebooks_user/models/notifications/app_notification.dart";
import "package:ebooks_user/models/search_result.dart";
import "package:ebooks_user/providers/notifications_provider.dart";
import "package:ebooks_user/screens/master_screen.dart";
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

  @override
  void initState() {
    super.initState();
    if (AuthProvider.isLoggedIn) {
      _notificationsProvider = context.read<NotificationsProvider>();
      _fetchNotifications();
    }
  }

  @override
  Widget build(BuildContext context) {
    Widget content;
    if (!AuthProvider.isLoggedIn) {
      content = Center(child: NotLoggedInView());
    } else if (_isLoading) {
      content = const Center(child: CircularProgressIndicator());
    } else if (_notifications?.count == 0) {
      content = const Center(child: Text("You don't have any notification"));
    } else {
      content = _buildResultView();
    }
    return MasterScreen(child: content);
  }

  Future _fetchNotifications() async {
    setState(() => _isLoading = true);
    try {
      final notifications = await _notificationsProvider.getPaged(
        page: _currentPage,
      );
      if (!mounted) return;
      setState(() => _notifications = notifications);
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

  Future _showDeleteDialog(int id) async {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text("Delete"),
        content: const Text(
          "Are you sure you want to delete this notification?",
        ),
        actions: [
          TextButton(
            onPressed: () async {
              Navigator.pop(context);
              await _notificationsProvider.delete(id);
              await _fetchNotifications();
            },
            child: const Text("Delete"),
          ),
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text("Cancel"),
          ),
        ],
      ),
    );
  }

  Widget _buildResultView() {
    final notifications = _notifications?.resultList ?? [];
    return ListView.builder(
      itemCount: notifications.length,
      itemBuilder: (context, index) {
        return Column(
          children: [
            ListTile(
              title: Text(
                notifications[index].message ?? "",
                style: TextStyle(
                  fontWeight: !notifications[index].isRead!
                      ? FontWeight.w700
                      : null,
                ),
              ),
              trailing:
                  notifications[index].bookId != null ||
                      notifications[index].publisherId != null
                  ? Icon(Icons.chevron_right_rounded)
                  : null,
              onTap: () async {
                if (!notifications[index].isRead!) {
                  await _notificationsProvider.markAsRead(
                    notifications[index].notificationId!,
                  );
                  await _fetchNotifications();
                }
                //navigate to the othet screen
              },
              onLongPress: () async {
                await _showDeleteDialog(notifications[index].notificationId!);
              },
            ),
            const Divider(height: 1),
          ],
        );
      },
    );
  }
}
