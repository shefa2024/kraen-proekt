// API Configuration
const API_CONFIG = {
    BASE_URL: window.location.origin + '/api',
    ENDPOINTS: {
        // Auth
        LOGIN: '/auth/login',
        REGISTER: '/auth/register',

        // Teachers
        TEACHERS: '/teachers',
        TEACHER_REVIEWS: (id) => `/teachers/${id}/reviews`,
        TEACHER_SCHEDULE: (id) => `/teachers/${id}/schedule`,
        UPDATE_PROFILE: '/teachers/profile',

        // Lessons
        MY_LESSONS: '/lessons/my-lessons',
        LESSON: (id) => `/lessons/${id}`,
        UPDATE_LESSON_STATUS: (id) => `/lessons/${id}/status`,

        // Reservations
        RESERVATIONS: '/reservations',
        MY_RESERVATIONS: '/reservations/my-reservations',
        RESERVATION: (id) => `/reservations/${id}`,
        UPDATE_RESERVATION_STATUS: (id) => `/reservations/${id}/status`,

        // Reviews
        REVIEWS: '/reviews',

        // Subjects
        SUBJECTS: '/subjects',

        // Messages
        MESSAGES: '/messages',
        CONVERSATIONS: '/messages/conversations',
        CONVERSATION: (userId) => `/messages/conversation/${userId}`,
        UNREAD_COUNT: '/messages/unread-count',

        // Admin
        ADMIN_DASHBOARD: '/admin/dashboard',
        ADMIN_USERS: '/admin/users',
        ADMIN_LESSONS: '/admin/lessons',
        ADMIN_RESERVATIONS: '/admin/reservations',
        TOGGLE_USER_ACTIVE: (id) => `/admin/users/${id}/toggle-active`
    }
};

// App State
const AppState = {
    currentUser: null,
    token: null,
    currentPage: 'home',

    setUser(user, token) {
        this.currentUser = user;
        this.token = token;
        localStorage.setItem('user', JSON.stringify(user));
        localStorage.setItem('token', token);
    },

    clearUser() {
        this.currentUser = null;
        this.token = null;
        localStorage.removeItem('user');
        localStorage.removeItem('token');
    },

    loadUser() {
        const user = localStorage.getItem('user');
        const token = localStorage.getItem('token');
        if (user && token) {
            this.currentUser = JSON.parse(user);
            this.token = token;
            return true;
        }
        return false;
    },

    isAuthenticated() {
        return !!this.token;
    },

    isStudent() {
        return this.currentUser?.role === 'Student';
    },

    isTeacher() {
        return this.currentUser?.role === 'Teacher';
    },

    isAdmin() {
        return this.currentUser?.role === 'Admin';
    },

    isParent() {
        return this.currentUser?.role === 'Parent';
    },

    isStudentOrParent() {
        return this.isStudent() || this.isParent();
    }
};

// Initialize app state
AppState.loadUser();
