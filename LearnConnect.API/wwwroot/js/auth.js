// Authentication Module
class AuthModule {
  constructor() {
    this.modal = document.getElementById('authModal');
    this.modalOverlay = document.getElementById('modalOverlay');
    this.modalClose = document.getElementById('modalClose');
    this.loginBtn = document.getElementById('loginBtn');
    this.signupBtn = document.getElementById('signupBtn');
    this.loginForm = document.getElementById('loginForm');
    this.signupForm = document.getElementById('signupForm');
    this.authTabs = document.querySelectorAll('.auth-tab');

    this.init();
  }

  init() {
    // Modal controls
    this.loginBtn?.addEventListener('click', () => this.openModal('login'));
    this.signupBtn?.addEventListener('click', () => this.openModal('signup'));
    this.modalClose?.addEventListener('click', () => this.closeModal());
    this.modalOverlay?.addEventListener('click', () => this.closeModal());

    // Tab switching
    this.authTabs.forEach(tab => {
      tab.addEventListener('click', () => {
        const tabName = tab.dataset.tab;
        this.switchTab(tabName);
      });
    });

    // Form submissions
    this.loginForm?.addEventListener('submit', (e) => this.handleLogin(e));
    this.signupForm?.addEventListener('submit', (e) => this.handleSignup(e));

    // Update UI based on auth state
    this.updateAuthUI();
  }

  openModal(tab = 'login') {
    this.modal?.classList.add('active');
    this.switchTab(tab);
    document.body.style.overflow = 'hidden';
  }

  closeModal() {
    this.modal?.classList.remove('active');
    document.body.style.overflow = '';
    this.clearErrors();
  }

  switchTab(tabName) {
    // Update tabs
    this.authTabs.forEach(tab => {
      if (tab.dataset.tab === tabName) {
        tab.classList.add('active');
      } else {
        tab.classList.remove('active');
      }
    });

    // Update forms
    const forms = document.querySelectorAll('.auth-form');
    forms.forEach(form => {
      if (form.id === `${tabName}Form`) {
        form.classList.add('active');
      } else {
        form.classList.remove('active');
      }
    });

    this.clearErrors();
  }

  async handleLogin(e) {
    e.preventDefault();

    const email = document.getElementById('loginEmail').value;
    const password = document.getElementById('loginPassword').value;
    const errorEl = document.getElementById('loginError');

    try {
      const response = await api.login(email, password);
      AppState.setUser(response, response.token);
      this.closeModal();
      this.updateAuthUI();

      // Redirect based on role
      if (AppState.isAdmin()) {
        window.app.navigate('admin');
      } else {
        window.app.navigate('dashboard');
      }

      this.showSuccess('Welcome back!');
    } catch (error) {
      this.showError(errorEl, error.message || 'Login failed');
    }
  }

  async handleSignup(e) {
    e.preventDefault();

    const data = {
      firstName: document.getElementById('signupFirstName').value,
      lastName: document.getElementById('signupLastName').value,
      email: document.getElementById('signupEmail').value,
      password: document.getElementById('signupPassword').value,
      role: document.getElementById('signupRole').value
    };
    const errorEl = document.getElementById('signupError');

    try {
      const response = await api.register(data);
      AppState.setUser(response, response.token);
      this.closeModal();
      this.updateAuthUI();

      // Redirect based on role
      window.app.navigate('dashboard');

      this.showSuccess('Account created successfully!');
    } catch (error) {
      this.showError(errorEl, error.message || 'Registration failed');
    }
  }

  showError(element, message) {
    if (element) {
      element.textContent = message;
      element.classList.add('active');
    }
  }

  clearErrors() {
    const errors = document.querySelectorAll('.form-error');
    errors.forEach(error => {
      error.textContent = '';
      error.classList.remove('active');
    });
  }

  showSuccess(message) {
    // Simple success notification
    const notification = document.createElement('div');
    notification.style.cssText = `
            position: fixed;
            top: 90px;
            right: 20px;
            background: linear-gradient(135deg, #06b6d4, #3b82f6);
            color: white;
            padding: 1rem 1.5rem;
            border-radius: 0.75rem;
            box-shadow: 0 10px 25px rgba(6, 182, 212, 0.3);
            z-index: 3000;
            animation: slideIn 0.3s ease;
        `;
    notification.textContent = message;
    document.body.appendChild(notification);

    setTimeout(() => {
      notification.style.animation = 'slideOut 0.3s ease';
      setTimeout(() => notification.remove(), 300);
    }, 3000);
  }

  updateAuthUI() {
    const navActions = document.querySelector('.nav-actions');
    const navLinks = document.querySelector('.nav-links');
    const mobileMenu = document.getElementById('mobileMenu');

    if (AppState.isAuthenticated()) {
      // Update deskop nav links
      if (navLinks) {
        navLinks.innerHTML = `
            <a href="#home" class="nav-link">Home</a>
            <a href="#teachers" class="nav-link">Find Teachers</a>
            <a href="#dashboard" class="nav-link" id="navDashboardLink">Dashboard</a>
        `;

        document.getElementById('navDashboardLink')?.addEventListener('click', (e) => {
          e.preventDefault();
          if (AppState.isAdmin()) {
            window.app.navigate('admin');
          } else {
            window.app.navigate('dashboard');
          }
        });
      }

      if (navActions) {
        navActions.innerHTML = `
                <button class="btn btn-text" id="messagesBtn">Messages</button>
                <button class="btn btn-text" id="meetingsBtn">Meetings</button>
                <button class="btn btn-secondary" id="logoutBtn">Logout</button>
            `;

        document.getElementById('messagesBtn')?.addEventListener('click', () => {
          window.app.navigate('messages');
        });

        document.getElementById('meetingsBtn')?.addEventListener('click', () => {
          window.app.navigate('meetings');
        });

        document.getElementById('logoutBtn')?.addEventListener('click', () => {
          AppState.clearUser();
          this.updateAuthUI();
          window.app.navigate('home');
          this.showSuccess('Logged out successfully');
        });
      }

      if (mobileMenu) {
        mobileMenu.innerHTML = `
            <a href="#home" class="mobile-link">Home</a>
            <a href="#teachers" class="mobile-link">Find Teachers</a>
            <a href="#dashboard" class="mobile-link">Dashboard</a>
            <a href="#messages" class="mobile-link">Messages</a>
            <button class="btn btn-secondary btn-block" id="mobileLogoutBtn">Logout</button>
        `;

        document.getElementById('mobileLogoutBtn')?.addEventListener('click', () => {
          AppState.clearUser();
          this.updateAuthUI();
          window.app.navigate('home');
          this.showSuccess('Logged out successfully');
        });

        mobileMenu.querySelectorAll('.mobile-link').forEach(link => {
          link.addEventListener('click', (e) => {
            const href = link.getAttribute('href');
            if (href && href.startsWith('#')) {
              e.preventDefault();
              const page = href.substring(1);
              window.app.navigate(page);
              mobileMenu.classList.remove('active');
            }
          });
        });
      }
    } else {
      if (navActions) {
        navActions.innerHTML = `
                <button class="btn btn-text" id="loginBtn">Log In</button>
                <button class="btn btn-primary" id="signupBtn">Sign Up</button>
            `;

        document.getElementById('loginBtn')?.addEventListener('click', () => this.openModal('login'));
        document.getElementById('signupBtn')?.addEventListener('click', () => this.openModal('signup'));
      }

      if (mobileMenu) {
        mobileMenu.innerHTML = `
            <a href="#home" class="mobile-link">Home</a>
            <a href="#teachers" class="mobile-link">Find Teachers</a>
            <button class="btn btn-text btn-block" id="mobileLoginBtn">Log In</button>
            <button class="btn btn-primary btn-block" id="mobileSignupBtn">Sign Up</button>
        `;

        document.getElementById('mobileLoginBtn')?.addEventListener('click', () => this.openModal('login'));
        document.getElementById('mobileSignupBtn')?.addEventListener('click', () => this.openModal('signup'));

        mobileMenu.querySelectorAll('.mobile-link').forEach(link => {
          link.addEventListener('click', (e) => {
            const href = link.getAttribute('href');
            if (href && href.startsWith('#')) {
              e.preventDefault();
              const page = href.substring(1);
              window.app.navigate(page);
              mobileMenu.classList.remove('active');
            }
          });
        });
      }
    }
  }

  logout() {
    AppState.clearUser();
    this.updateAuthUI();
    window.app.navigate('home');
  }
}

// Initialize auth module when DOM is ready
let authModule;
document.addEventListener('DOMContentLoaded', () => {
  authModule = new AuthModule();
});
