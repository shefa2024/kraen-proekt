// API Service
class ApiService {
  constructor() {
    this.baseUrl = API_CONFIG.BASE_URL;
  }

  async request(endpoint, options = {}) {
    const url = `${this.baseUrl}${endpoint}`;
    const headers = {
      'Content-Type': 'application/json',
      ...options.headers
    };

    if (AppState.token) {
      headers['Authorization'] = `Bearer ${AppState.token}`;
    }

    try {
      const response = await fetch(url, {
        ...options,
        headers
      });

      if (response.status === 401) {
        AppState.clearUser();
        window.location.reload();
        throw new Error('Unauthorized');
      }

      const data = await response.json().catch(() => null);

      if (!response.ok) {
        let errorMessage = data?.message || data?.error || 'Request failed';
        if (data?.errors) {
          errorMessage = Object.values(data.errors).flat().join(', ');
        }
        throw new Error(errorMessage);
      }

      return data;
    } catch (error) {
      console.error('API Error:', error);
      throw error;
    }
  }

  // Auth
  async login(email, password) {
    return this.request(API_CONFIG.ENDPOINTS.LOGIN, {
      method: 'POST',
      body: JSON.stringify({ email, password })
    });
  }

  async register(data) {
    return this.request(API_CONFIG.ENDPOINTS.REGISTER, {
      method: 'POST',
      body: JSON.stringify(data)
    });
  }

  // Teachers
  async getTeachers(params = {}) {
    const queryString = new URLSearchParams(params).toString();
    const endpoint = queryString
      ? `${API_CONFIG.ENDPOINTS.TEACHERS}?${queryString}`
      : API_CONFIG.ENDPOINTS.TEACHERS;
    return this.request(endpoint);
  }

  async getTeacher(id) {
    return this.request(`${API_CONFIG.ENDPOINTS.TEACHERS}/${id}`);
  }

  async updateTeacherProfile(data) {
    return this.request(API_CONFIG.ENDPOINTS.UPDATE_PROFILE, {
      method: 'PUT',
      body: JSON.stringify(data)
    });
  }

  async getTeacherReviews(id) {
    return this.request(API_CONFIG.ENDPOINTS.TEACHER_REVIEWS(id));
  }

  async getTeacherSchedule(id) {
    return this.request(API_CONFIG.ENDPOINTS.TEACHER_SCHEDULE(id));
  }

  async getMySchedule() {
    return this.request('/Teachers/my-schedule');
  }

  async updateTeacherSchedule(data) {
    return this.request('/Teachers/schedule', {
      method: 'PUT',
      body: JSON.stringify(data)
    });
  }

  async submitVerification(certificateUrl) {
    return this.request('/Teachers/verify', {
      method: 'POST',
      body: JSON.stringify(certificateUrl)
    });
  }

  // Lessons
  async getMyLessons() {
    return this.request(API_CONFIG.ENDPOINTS.MY_LESSONS);
  }

  async getLesson(id) {
    return this.request(API_CONFIG.ENDPOINTS.LESSON(id));
  }

  async updateLessonStatus(id, status) {
    return this.request(API_CONFIG.ENDPOINTS.UPDATE_LESSON_STATUS(id), {
      method: 'PUT',
      body: JSON.stringify(status)
    });
  }

  // Reservations
  async createReservation(data) {
    return this.request(API_CONFIG.ENDPOINTS.RESERVATIONS, {
      method: 'POST',
      body: JSON.stringify(data)
    });
  }

  async getMyReservations() {
    return this.request(API_CONFIG.ENDPOINTS.MY_RESERVATIONS);
  }

  async getReservation(id) {
    return this.request(API_CONFIG.ENDPOINTS.RESERVATION(id));
  }

  async updateReservationStatus(id, status) {
    return this.request(API_CONFIG.ENDPOINTS.UPDATE_RESERVATION_STATUS(id), {
      method: 'PUT',
      body: JSON.stringify({ status })
    });
  }

  // Reviews
  async createReview(data) {
    return this.request(API_CONFIG.ENDPOINTS.REVIEWS, {
      method: 'POST',
      body: JSON.stringify(data)
    });
  }

  // Subjects
  async getSubjects() {
    return this.request(API_CONFIG.ENDPOINTS.SUBJECTS);
  }

  // Messages
  async getConversations() {
    return this.request(API_CONFIG.ENDPOINTS.CONVERSATIONS);
  }

  async getConversation(userId) {
    return this.request(API_CONFIG.ENDPOINTS.CONVERSATION(userId));
  }

  async sendMessage(receiverId, content) {
    return this.request(API_CONFIG.ENDPOINTS.MESSAGES, {
      method: 'POST',
      body: JSON.stringify({ receiverId, content })
    });
  }

  async getUnreadCount() {
    return this.request(API_CONFIG.ENDPOINTS.UNREAD_COUNT);
  }

  // Admin
  async getDashboardStats() {
    return this.request(API_CONFIG.ENDPOINTS.ADMIN_DASHBOARD);
  }

  async getAllUsers() {
    return this.request(API_CONFIG.ENDPOINTS.ADMIN_USERS);
  }

  async getAllLessons() {
    return this.request(API_CONFIG.ENDPOINTS.ADMIN_LESSONS);
  }

  async getAllReservations() {
    return this.request(API_CONFIG.ENDPOINTS.ADMIN_RESERVATIONS);
  }

  async toggleUserActive(id) {
    return this.request(API_CONFIG.ENDPOINTS.TOGGLE_USER_ACTIVE(id), {
      method: 'PUT'
    });
  }

  // Notebooks
  async getOrCreateNotebook(studentId, teacherId) {
    return this.request(`/Notebooks/student/${studentId}/teacher/${teacherId}`);
  }

  async getOrCreateNotebookByLesson(param) {
    if (String(param).includes('teacher-')) {
      const teacherId = String(param).replace('teacher-', '');
      return this.request(`/Notebooks/student/${AppState.currentUser.id}/teacher/${teacherId}`);
    } else if (String(param).includes('student-')) {
      const studentId = String(param).replace('student-', '');
      return this.request(`/Notebooks/student/${studentId}/teacher/${AppState.currentUser.id}`);
    } else {
      // It's a lesson ID
      return this.request(`/Notebooks/lesson/${param}`);
    }
  }

  async updateNotebook(id, content) {
    return this.request(`/Notebooks/${id}`, {
      method: 'PUT',
      body: JSON.stringify(content)
    });
  }

  // Meetings
  async getMyMeetings() {
    return this.request('/Meetings/my-meetings');
  }

  async createMeetingRequest(data) {
    return this.request('/Meetings', {
      method: 'POST',
      body: JSON.stringify(data)
    });
  }

  async updateMeetingStatus(id, status) {
    return this.request(`/Meetings/${id}/status`, {
      method: 'PUT',
      body: JSON.stringify(status)
    });
  }

  // Gamification & Reports
  async getWeeklyReport(studentId) {
    return this.request(`/Reports/student/${studentId}/weekly`);
  }

  async getStudentStats(studentId) {
    return this.request(`/Auth/student-stats/${studentId}`);
  }

  // ── Teacher Verification (Admin) ──────────────────────────────────────────
  async getVerifications(status = 'Pending') {
    return this.request(`/Admin/verifications?status=${status}`);
  }

  async updateVerification(teacherId, status, notes = '') {
    return this.request(`/Admin/verifications/${teacherId}`, {
      method: 'PUT',
      body: JSON.stringify({ status, notes })
    });
  }

  // ── Parent API ────────────────────────────────────────────────────────────
  async getMyChildren() {
    return this.request('/Parent/children');
  }

  async linkChild(childEmail) {
    return this.request('/Parent/children/link', {
      method: 'POST',
      body: JSON.stringify({ childEmail })
    });
  }

  async unlinkChild(studentId) {
    return this.request(`/Parent/children/${studentId}`, {
      method: 'DELETE'
    });
  }

  async bookForChild(studentId, data) {
    return this.request(`/Parent/children/${studentId}/reservations`, {
      method: 'POST',
      body: JSON.stringify(data)
    });
  }

  async getChildrenReservations() {
    return this.request('/Parent/children/reservations');
  }

  // ── Video Call ────────────────────────────────────────────────────────────
  async getVideoCallRoom(reservationId) {
    return this.request(`/VideoCall/room/${reservationId}`);
  }

  // ── Payments ────────────────────────────────────────────────────────────
  async processPayment(data) {
    return this.request('/Payments/process', {
      method: 'POST',
      body: JSON.stringify(data)
    });
  }

  async createPaymentIntent(data) {
    return this.request('/Payments/create-payment-intent', {
      method: 'POST',
      body: JSON.stringify(data)
    });
  }

  async confirmPaymentInDb(paymentId) {
    return this.request(`/Payments/confirm/${paymentId}`, {
      method: 'POST'
    });
  }
}

const api = new ApiService();
