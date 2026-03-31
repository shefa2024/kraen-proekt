// Main Application
class App {
  constructor() {
    this.container = document.getElementById('app');
    this.currentPage = 'home';
    this.subjects = [];
    this.stripe = Stripe('pk_test_51TDp6CRtJFgXdvuM42kuYEAwxbefUHGytYpWjaotISpbMVVUTZMz9To5oL6jUVctjZ8PquL2cxzKLxMTAPCtuLFw00P3irzDo9');
    this.stripeElements = null;
    this.init();
  }

  async init() {
    // Load subjects
    try {
      this.subjects = await api.getSubjects();
    } catch (error) {
      console.error('Failed to load subjects:', error);
    }

    // Setup navigation
    this.setupNavigation();

    // Setup mobile menu
    this.setupMobileMenu();

    // Handle initial route
    this.handleRoute();

    // Listen for hash changes
    window.addEventListener('hashchange', () => this.handleRoute());
  }

  setupNavigation() {
    const navLinks = document.querySelectorAll('a[href^="#"]');
    navLinks.forEach(link => {
      link.addEventListener('click', (e) => {
        const href = link.getAttribute('href');
        if (href && href.startsWith('#')) {
          e.preventDefault();
          const page = href.substring(1);
          
          // Close any open modals
          if (typeof authModule !== 'undefined') {
            authModule.closeModal();
          }
          
          this.navigate(page);
        }
      });
    });

    // Navbar scroll effect
    window.addEventListener('scroll', () => {
      const navbar = document.getElementById('navbar');
      if (window.scrollY > 50) {
        navbar?.classList.add('scrolled');
      } else {
        navbar?.classList.remove('scrolled');
      }
    });
  }

  setupMobileMenu() {
    const mobileMenuBtn = document.getElementById('mobileMenuBtn');
    const mobileMenu = document.getElementById('mobileMenu');

    mobileMenuBtn?.addEventListener('click', () => {
      mobileMenu?.classList.toggle('active');
    });

    // Close mobile menu when clicking a link
    const mobileLinks = document.querySelectorAll('.mobile-link');
    mobileLinks.forEach(link => {
      link.addEventListener('click', () => {
        mobileMenu?.classList.remove('active');
      });
    });
  }

  handleRoute() {
    const hash = window.location.hash.substring(1) || 'home';
    const parts = hash.split('/');
    const page = parts[0];
    const id = parts[1];

    this.navigate(page, id);
  }

  navigate(page, id = null) {
    // Cleanup video call if moving away from it
    if (this.currentPage === 'videocall' && page !== 'videocall') {
      if (typeof videoCallManager !== 'undefined') {
        videoCallManager.cleanup();
      }
    }

    this.currentPage = page;
    window.location.hash = id ? `${page}/${id}` : page;

    // Update active nav link
    document.querySelectorAll('.nav-link').forEach(link => {
      const href = link.getAttribute('href');
      if (href === `#${page}`) {
        link.classList.add('active');
      } else {
        link.classList.remove('active');
      }
    });

    // Render page
    this.renderPage(page, id);
  }

  async renderPage(page, id) {
    switch (page) {
      case 'home':
        this.renderHome();
        break;
      case 'teachers':
        this.renderTeachers();
        break;
      case 'teacher':
        if (id) this.renderTeacherProfile(id);
        break;
      case 'dashboard':
        if (AppState.isAuthenticated()) {
          this.renderDashboard();
        } else {
          this.navigate('home');
        }
        break;
      case 'admin':
        if (AppState.isAdmin()) {
          this.renderAdmin();
        } else {
          this.navigate('home');
        }
        break;

      case 'messages':
        if (id) {
          this.renderMessages(id);
        } else {
          this.renderMessages();
        }
        break;
      case 'booking':
        if (id) this.renderBooking(id);
        break;
      case 'booking-confirmation':
        this.renderBookingConfirmation(id);
        break;
      case 'whiteboard':
        this.renderWhiteboard(id);
        break;
      case 'notebook':
        this.renderNotebook(id);
        break;
      case 'meetings':
        this.renderMeetings();
        break;
      case 'videocall':
        if (id) {
          // Sanitize: extract only the numeric part (in case of malformed hash like '10:1')
          const numericId = String(id).match(/\d+/)?.[0];
          if (numericId) videoCallManager.joinRoom(numericId);
        }
        break;
      case 'privacy':
        this.renderPrivacyPolicy();
        break;
      case 'terms':
        this.renderTermsOfService();
        break;
      default:
        this.renderHome();
    }
  }

  renderHome() {
    this.container.innerHTML = `
            <section class="hero">
                <div class="container">
                    <div class="hero-content">
                        <div class="hero-text">
                            <h1>Learn Anything, Anytime with Expert Teachers</h1>
                            <p>Connect with qualified teachers for personalized one-on-one lessons. Master new skills at your own pace with flexible scheduling.</p>
                            <div class="hero-actions">
                                <button class="btn btn-primary btn-lg" onclick="window.app.navigate('teachers')">Find a Teacher</button>
                            </div>
                        </div>
                        <div class="hero-image">
                            <img src="logo.png" alt="PrivateExtracurriculars Illustration">
                        </div>
                    </div>
                </div>
            </section>

            <section class="stats">
                <div class="container">
                    <div class="stats-grid">
                        <div class="stat-card">
                            <div class="stat-number">10,000+</div>
                            <div class="stat-label">Active Students</div>
                        </div>
                        <div class="stat-card">
                            <div class="stat-number">5,000+</div>
                            <div class="stat-label">Expert Teachers</div>
                        </div>
                        <div class="stat-card">
                            <div class="stat-number">50+</div>
                            <div class="stat-label">Subjects</div>
                        </div>
                        <div class="stat-card">
                            <div class="stat-number">4.9/5</div>
                            <div class="stat-label">Average Rating</div>
                        </div>
                    </div>
                </div>
            </section>

            <section class="teachers-section" id="featured-teachers">
                <div class="container">
                    <div class="section-header">
                        <h2 class="section-title">Featured Teachers</h2>
                        <p class="section-subtitle">Discover our top-rated educators ready to help you achieve your learning goals</p>
                    </div>
                    <div id="featured-teachers-grid"></div>
                    <div style="text-align: center; margin-top: 2rem;">
                        <button class="btn btn-primary btn-lg" onclick="window.app.navigate('teachers')">View All Teachers</button>
                    </div>
                </div>
            </section>
        `;

    this.loadFeaturedTeachers();
  }

  async loadFeaturedTeachers() {
    const grid = document.getElementById('featured-teachers-grid');
    if (!grid) return;

    grid.innerHTML = '<div class="loading-container"><div class="spinner"></div></div>';

    try {
      const teachers = await api.getTeachers({ pageSize: 6 });
      grid.className = 'teachers-grid';
      grid.innerHTML = teachers.map(teacher => this.createTeacherCard(teacher)).join('');
    } catch (error) {
      grid.innerHTML = '<p style="text-align: center; color: var(--gray-500);">Failed to load teachers</p>';
    }
  }

  async renderTeachers() {
    this.container.innerHTML = `
            <section class="teachers-section" style="padding-top: 120px;">
                <div class="container">
                    <div class="section-header">
                        <h2 class="section-title">Find Your Perfect Teacher</h2>
                        <p class="section-subtitle">Search and filter through our expert educators</p>
                    </div>

                    <div class="search-filters">
                        <input type="text" class="search-input" id="searchInput" placeholder="Search by name or subject...">
                        <select class="filter-select" id="subjectFilter">
                            <option value="">All Subjects</option>
                            ${this.subjects.map(s => `<option value="${s.name}">${s.name}</option>`).join('')}
                        </select>
                        <select class="filter-select" id="priceFilter">
                            <option value="">All Prices</option>
                            <option value="0-30">$0 - $30</option>
                            <option value="30-50">$30 - $50</option>
                            <option value="50-100">$50 - $100</option>
                            <option value="100+">$100+</option>
                        </select>
                        <select class="filter-select" id="ratingFilter">
                            <option value="">All Ratings</option>
                            <option value="4.5">4.5+ Stars</option>
                            <option value="4">4+ Stars</option>
                            <option value="3">3+ Stars</option>
                        </select>
                    </div>

                    <div id="teachers-grid"></div>
                </div>
            </section>
        `;

    this.setupTeacherFilters();
    this.loadTeachers();
  }

  setupTeacherFilters() {
    const searchInput = document.getElementById('searchInput');
    const subjectFilter = document.getElementById('subjectFilter');
    const priceFilter = document.getElementById('priceFilter');
    const ratingFilter = document.getElementById('ratingFilter');

    const applyFilters = () => {
      const params = {};

      if (searchInput?.value) params.searchTerm = searchInput.value;
      if (subjectFilter?.value) params.subject = subjectFilter.value;
      if (ratingFilter?.value) params.minRating = parseFloat(ratingFilter.value);

      if (priceFilter?.value) {
        const [min, max] = priceFilter.value.split('-');
        if (min) params.minPrice = parseFloat(min);
        if (max && max !== '+') params.maxPrice = parseFloat(max);
      }

      this.loadTeachers(params);
    };

    searchInput?.addEventListener('input', () => {
      clearTimeout(this.searchTimeout);
      this.searchTimeout = setTimeout(applyFilters, 500);
    });

    subjectFilter?.addEventListener('change', applyFilters);
    priceFilter?.addEventListener('change', applyFilters);
    ratingFilter?.addEventListener('change', applyFilters);
  }

  async loadTeachers(params = {}) {
    const grid = document.getElementById('teachers-grid');
    if (!grid) return;

    grid.innerHTML = '<div class="loading-container"><div class="spinner"></div></div>';

    try {
      const teachers = await api.getTeachers({ ...params, pageSize: 20 });
      grid.className = 'teachers-grid';

      if (teachers.length === 0) {
        grid.innerHTML = '<p style="text-align: center; color: var(--gray-500); grid-column: 1/-1;">No teachers found</p>';
      } else {
        grid.innerHTML = teachers.map(teacher => this.createTeacherCard(teacher)).join('');
      }
    } catch (error) {
      grid.innerHTML = '<p style="text-align: center; color: var(--error); grid-column: 1/-1;">Failed to load teachers</p>';
    }
  }

  createTeacherCard(teacher) {
    const initials = `${teacher.firstName[0]}${teacher.lastName[0]}`;
    const rating = teacher.averageRating > 0 ? teacher.averageRating.toFixed(1) : 'New';

    return `
            <div class="teacher-card" onclick="window.app.navigate('teacher', ${teacher.id})">
                <div class="teacher-header">
                    <div class="teacher-avatar">${initials}</div>
                    <div class="teacher-info">
                        <div class="teacher-name">${teacher.firstName} ${teacher.lastName} ${teacher.isVerified ? '<span style="color:#22c55e;font-size:0.85rem;" title="Verified Teacher">✓</span>' : ''}</div>
                        <div class="teacher-rating">
                            ★ ${rating} ${teacher.totalReviews > 0 ? `(${teacher.totalReviews})` : ''}
                        </div>
                        ${teacher.location ? `<div class="teacher-location">📍 ${teacher.location}</div>` : ''}
                    </div>
                </div>
                <div class="teacher-subjects">
                    ${teacher.subjects.slice(0, 3).map(s => `<span class="subject-tag">${s}</span>`).join('')}
                </div>
                <div class="teacher-bio">${teacher.bio || 'Experienced educator ready to help you succeed.'}</div>
                <div class="teacher-footer">
                    <div class="teacher-price">$${teacher.hourlyRate}<span>/hour</span></div>
                    <button class="btn btn-primary btn-sm" onclick="event.stopPropagation(); window.app.navigate('teacher', ${teacher.id})">View Profile</button>
                </div>
            </div>
        `;
  }

  async renderTeacherProfile(id) {
    this.container.innerHTML = '<div class="loading-container"><div class="spinner"></div></div>';

    try {
      const teacher = await api.getTeacher(id);
      const reviews = await api.getTeacherReviews(id);

      const initials = `${teacher.firstName[0]}${teacher.lastName[0]}`;
      const rating = teacher.averageRating > 0 ? teacher.averageRating.toFixed(1) : 'New';

      this.container.innerHTML = `
                <section class="profile-section">
                    <div class="container">
                        <div class="profile-header">
                            <div class="profile-top">
                                <div class="profile-avatar">${initials}</div>
                                <div class="profile-main">
                                    <h1 class="profile-name">${teacher.firstName} ${teacher.lastName}</h1>
                                    <div class="profile-meta">
                                        <div class="profile-meta-item">★ ${rating} (${teacher.totalReviews} reviews)</div>
                                        ${teacher.location ? `<div class="profile-meta-item">📍 ${teacher.location}</div>` : ''}
                                        ${teacher.memberSince ? `<div class="profile-meta-item">👤 Member since ${teacher.memberSince}</div>` : ''}
                                    </div>
                                    <div class="teacher-subjects">
                                        ${teacher.subjects.map(s => `<span class="subject-tag">${s}</span>`).join('')}
                                    </div>
                                </div>
                            </div>
                            <div class="profile-stats">
                                <div class="profile-stat">
                                    <span class="profile-stat-value">${teacher.totalLessons}+</span>
                                    <span class="profile-stat-label">Total Lessons</span>
                                </div>
                                <div class="profile-stat">
                                    <span class="profile-stat-value">${teacher.responseTime || 'Fast'}</span>
                                    <span class="profile-stat-label">Response Time</span>
                                </div>
                                <div class="profile-stat">
                                    <span class="profile-stat-value">${teacher.yearsOfExperience}+</span>
                                    <span class="profile-stat-label">Years Experience</span>
                                </div>
                            </div>
                        </div>

                        <div class="profile-content">
                            <div>
                                <div class="profile-about">
                                    <h2>About</h2>
                                    <p>${teacher.bio || 'Experienced educator ready to help you succeed.'}</p>
                                    
                                    ${teacher.education ? `
                                        <h3 style="margin-top: 2rem;">Education</h3>
                                        <p>${teacher.education}</p>
                                    ` : ''}
                                    
                                    ${teacher.languages && teacher.languages.length > 0 ? `
                                        <h3 style="margin-top: 2rem;">Languages</h3>
                                        <p>${teacher.languages.join(', ')}</p>
                                    ` : ''}
                                </div>

                                ${reviews.length > 0 ? `
                                    <div class="profile-about reviews-section" style="margin-top: 1.5rem;">
                                        <h2>Reviews</h2>
                                        ${reviews.slice(0, 5).map(review => `
                                            <div class="review-card">
                                                <div class="review-header">
                                                    <span class="review-author">${review.studentName}</span>
                                                    <span class="review-date">${new Date(review.createdAt).toLocaleDateString()}</span>
                                                </div>
                                                <div class="review-rating">${'★'.repeat(review.rating)}${'☆'.repeat(5 - review.rating)}</div>
                                                ${review.comment ? `<p class="review-comment">${review.comment}</p>` : ''}
                                            </div>
                                        `).join('')}
                                    </div>
                                ` : ''}
                            </div>

                            <div class="profile-sidebar">
                                <div class="sidebar-card booking-card">
                                    <div class="booking-header">
                                        <h3>Book a Lesson</h3>
                                        <div class="price-display">$${teacher.hourlyRate}<span style="font-size: 1rem; color: var(--gray-500);">/hour</span></div>
                                    </div>
                                    <div class="booking-features">
                                        <div class="booking-feature">
                                            <span class="feature-icon">✓</span>
                                            <span>Personalized 1-on-1 lessons</span>
                                        </div>
                                        <div class="booking-feature">
                                            <span class="feature-icon">✓</span>
                                            <span>Flexible scheduling</span>
                                        </div>
                                        <div class="booking-feature">
                                            <span class="feature-icon">✓</span>
                                            <span>Free cancellation</span>
                                        </div>
                                    </div>
                                    ${AppState.isStudentOrParent() ? `
                                        <button class="btn btn-primary btn-block btn-book" onclick="window.app.navigate('booking', ${teacher.id})">
                                            <span class="btn-icon">📅</span> Book a Lesson
                                        </button>
                                        <button class="btn btn-secondary btn-block" style="margin-top: 0.5rem;" onclick="window.app.navigate('messages', ${teacher.id})">
                                            <span class="btn-icon">💬</span> Send Message
                                        </button>
                                    ` : AppState.isAuthenticated() ? `
                                        <div class="booking-notice">
                                            <p>Log in as a student to book lessons with this teacher.</p>
                                            <button class="btn btn-secondary btn-block" onclick="authModule.logout()">Switch Account</button>
                                        </div>
                                    ` : `
                                        <button class="btn btn-primary btn-block btn-book" onclick="authModule.openModal('login')">
                                            <span class="btn-icon">🔐</span> Login to Book
                                        </button>
                                        <button class="btn btn-secondary btn-block" style="margin-top: 0.5rem;" onclick="authModule.openModal('signup')">
                                            <span class="btn-icon">✨</span> Create Account
                                        </button>
                                        <p class="booking-hint">New to LearnConnect? Sign up free!</p>
                                    `}
                                </div>

                                <div class="sidebar-card">
                                    <h3>Quick Facts</h3>
                                    <div class="quick-facts">
                                        <div class="quick-fact">
                                            <span class="quick-fact-label">Response time</span>
                                            <span class="quick-fact-value">${teacher.responseTime || 'Within 2 hours'}</span>
                                        </div>
                                        <div class="quick-fact">
                                            <span class="quick-fact-label">Total lessons</span>
                                            <span class="quick-fact-value">${teacher.totalLessons}+</span>
                                        </div>
                                        <div class="quick-fact">
                                            <span class="quick-fact-label">Experience</span>
                                            <span class="quick-fact-value">${teacher.yearsOfExperience} years</span>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </section>
            `;
    } catch (error) {
      this.container.innerHTML = '<div class="container" style="padding: 120px 0;"><p style="text-align: center; color: var(--error);">Failed to load teacher profile</p></div>';
    }
  }

  bookLesson(teacherId) {
    if (!AppState.isStudentOrParent()) {
      authModule.openModal('signup');
      return;
    }

    // Create booking modal
    const modal = document.createElement('div');
    modal.className = 'modal active';
    modal.innerHTML = `
            <div class="modal-overlay"></div>
            <div class="modal-content">
                <button class="modal-close">&times;</button>
                <div class="auth-container">
                    <h2>Book a Lesson</h2>
                    <form id="bookingForm">
                        <div class="form-group">
                            <label>Preferred Date & Time</label>
                            <input type="datetime-local" class="form-input" id="lessonDateTime" required>
                        </div>
                        <div class="form-group">
                            <label>Duration (minutes)</label>
                            <select class="form-input" id="lessonDuration">
                                <option value="60">60 minutes</option>
                                <option value="90">90 minutes</option>
                                <option value="120">120 minutes</option>
                            </select>
                        </div>
                        <div class="form-group">
                            <label>Message (optional)</label>
                            <textarea class="form-input" id="lessonMessage" rows="3" placeholder="Tell the teacher about your learning goals..."></textarea>
                        </div>
                        <div class="form-error" id="bookingError"></div>
                        <button type="submit" class="btn btn-primary btn-block">Send Booking Request</button>
                    </form>
                </div>
            </div>
        `;

    document.body.appendChild(modal);
    document.body.style.overflow = 'hidden';

    const closeModal = () => {
      modal.remove();
      document.body.style.overflow = '';
    };

    modal.querySelector('.modal-close').addEventListener('click', closeModal);
    modal.querySelector('.modal-overlay').addEventListener('click', closeModal);

    modal.querySelector('#bookingForm').addEventListener('submit', async (e) => {
      e.preventDefault();

      const data = {
        teacherId: teacherId,
        requestedDateTime: new Date(document.getElementById('lessonDateTime').value).toISOString(),
        durationMinutes: parseInt(document.getElementById('lessonDuration').value),
        message: document.getElementById('lessonMessage').value
      };

      try {
        await api.createReservation(data);
        closeModal();
        authModule.showSuccess('Booking request sent successfully!');
        this.navigate('dashboard');
      } catch (error) {
        const errorEl = document.getElementById('bookingError');
        errorEl.textContent = error.message || 'Booking failed';
        errorEl.classList.add('active');
      }
    });
  }

  async renderBooking(teacherId) {
    if (!AppState.isStudentOrParent()) {
      authModule.openModal('signup');
      return;
    }

    this.container.innerHTML = '<div class="loading-container"><div class="spinner"></div></div>';

    try {
      const teacher = await api.getTeacher(teacherId);

      // Initialize booking state
      this.bookingState = {
        teacherId,
        teacher,
        date: new Date(),
        selectedSlot: null,
        recurring: false,
        duration: 60
      };

      this.renderBookingView(teacher);
    } catch (error) {
      this.container.innerHTML = '<div class="container" style="padding: 120px 0;"><p style="text-align: center; color: var(--error);">Failed to load booking page</p></div>';
    }
  }

  renderBookingView(teacher) {
    const months = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];
    const currentMonth = months[this.bookingState.date.getMonth()];
    const currentYear = this.bookingState.date.getFullYear();

    this.container.innerHTML = `
      <section class="booking-section">
        <div class="container">
          <div class="section-header">
            <h2 class="section-title">Book a Lesson with ${teacher.firstName}</h2>
            <p class="section-subtitle">Select a date and time for your lesson</p>
          </div>

          <div class="booking-container">
            <!-- Calendar & Slots -->
            <div class="booking-main">
              <div class="calendar-card">
                <div class="calendar-header">
                  <h3>${currentMonth} ${currentYear}</h3>
                  <div class="calendar-controls">
                    <button class="btn btn-secondary btn-sm" onclick="window.app.changeBookingMonth(-1)"><</button>
                    <button class="btn btn-secondary btn-sm" onclick="window.app.changeBookingMonth(1)">></button>
                  </div>
                </div>
                
                <div class="calendar-grid">
                  <div class="calendar-day-header">Sun</div>
                  <div class="calendar-day-header">Mon</div>
                  <div class="calendar-day-header">Tue</div>
                  <div class="calendar-day-header">Wed</div>
                  <div class="calendar-day-header">Thu</div>
                  <div class="calendar-day-header">Fri</div>
                  <div class="calendar-day-header">Sat</div>
                  ${this.generateCalendarDays()}
                </div>

                <div class="time-slots" id="timeSlots">
                  <p style="grid-column: 1/-1; text-align: center; color: var(--gray-500);">Select a date to see available times</p>
                </div>
              </div>
            </div>

            <!-- Booking Summary -->
            <div class="booking-sidebar">
              <div class="booking-summary">
                <h3>Booking Summary</h3>
                <div class="booking-details">
                   <div class="booking-detail-row">
                    <span>Teacher</span>
                    <span>${teacher.firstName} ${teacher.lastName}</span>
                   </div>
                   <div class="booking-detail-row">
                    <span>Rate</span>
                    <span>$${teacher.hourlyRate}/hr</span>
                   </div>
                   <div class="booking-detail-row">
                    <span>Duration</span>
                    <span>
                      <select class="form-input" style="padding: 0.25rem; width: auto;" onchange="window.app.updateBookingDuration(this.value)">
                        <option value="60">60 min</option>
                        <option value="90">90 min</option>
                        <option value="120">120 min</option>
                      </select>
                    </span>
                   </div>
                   
                   <div class="form-group" style="margin-top: 1rem;">
                    <label class="checkbox-label" style="display: flex; align-items: center; gap: 0.5rem; cursor: pointer;">
                      <input type="checkbox" onchange="window.app.toggleRecurring(this.checked)">
                      <span>Make this a recurring lesson</span>
                    </label>
                   </div>

                   <div class="booking-total">
                    <span>Total</span>
                    <span id="bookingTotal">$${teacher.hourlyRate}</span>
                   </div>

                   <div class="payment-section" style="margin-top: 1.5rem;">
                     <h4 style="font-size: 0.875rem; color: var(--gray-700); margin-bottom: 0.75rem;">Select Payment Method</h4>
                     <div class="payment-methods">
                        <label class="payment-method active" onclick="window.app.selectPaymentMethod(this)">
                            <input type="radio" name="payment" value="card" checked>
                            <span class="payment-label">
                                <span style="font-size: 1.25rem;">💳</span>
                                <span>Credit / Debit Card</span>
                            </span>
                        </label>
                        <label class="payment-method" onclick="window.app.selectPaymentMethod(this)">
                            <input type="radio" name="payment" value="paypal">
                            <span class="payment-label">
                                <span style="font-size: 1.25rem;">🅿️</span>
                                <span>PayPal Checkout</span>
                            </span>
                        </label>
                        <label class="payment-method" onclick="window.app.selectPaymentMethod(this)">
                            <input type="radio" name="payment" value="apple">
                            <span class="payment-label">
                                <span style="font-size: 1.25rem;">🍎</span>
                                <span>Apple Pay</span>
                            </span>
                        </label>
                     </div>
                   </div>
                </div>

                <button class="btn btn-primary btn-block btn-lg" id="confirmBookingBtn" onclick="window.app.confirmBooking()" disabled style="margin-top: 1.5rem;">
                  Confirm & Pay Now
                </button>
              </div>
            </div>
          </div>
        </div>
      </section>
    `;
  }

  generateCalendarDays() {
    const year = this.bookingState.date.getFullYear();
    const month = this.bookingState.date.getMonth();
    const firstDay = new Date(year, month, 1).getDay();
    const daysInMonth = new Date(year, month + 1, 0).getDate();

    let html = '';

    // Empty slots for previous month
    for (let i = 0; i < firstDay; i++) {
      html += '<div class="calendar-day disabled"></div>';
    }

    // Days
    const today = new Date();
    for (let i = 1; i <= daysInMonth; i++) {
      const date = new Date(year, month, i);
      const isPast = date < new Date(today.setHours(0, 0, 0, 0));
      const isDisabled = isPast; // In real app, check schedule
      const hasSlots = !isDisabled && Math.random() > 0.3; // Mock availability

      html += `
            <div class="calendar-day ${isDisabled ? 'disabled' : ''} ${hasSlots ? 'has-slots' : ''}" 
                 onclick="${!isDisabled ? `window.app.selectBookingDate(${i})` : ''}">
                ${i}
            </div>
        `;
    }

    return html;
  }

  changeBookingMonth(delta) {
    this.bookingState.date.setMonth(this.bookingState.date.getMonth() + delta);
    this.navigate('booking', this.bookingState.teacherId);
  }

  selectBookingDate(day) {
    this.bookingState.selectedDay = day;
    this.bookingState.selectedSlot = null;
    if (document.getElementById('confirmBookingBtn'))
      document.getElementById('confirmBookingBtn').disabled = true;

    // Update UI for selected day
    document.querySelectorAll('.calendar-day').forEach(el => el.classList.remove('active'));
    event.target.closest('.calendar-day').classList.add('active');

    this._refreshTimeSlots();
    document.getElementById('timeSlots').scrollIntoView({ behavior: 'smooth' });
  }

  _refreshTimeSlots() {
    const slotsDiv = document.getElementById('timeSlots');
    if (!slotsDiv) return;

    // Fixed available hours 9-17 (for 120min last slot is 16:00)
    const duration = this.bookingState.duration;
    const lastHour = duration === 120 ? 16 : 17;

    // Simulate booked slots (in a real app these come from the server)
    const bookedHours = this.bookingState.bookedHours || [];

    let slotsHtml = '';
    for (let h = 9; h <= lastHour; h++) {
      const isBooked = bookedHours.includes(h) || (duration === 120 && bookedHours.includes(h + 1));
      const label = `${h}:00`;
      const next = duration === 120 ? ` – ${h + 2}:00` : '';
      if (isBooked) {
        slotsHtml += `<div class="time-slot time-slot-booked">${label}${next}</div>`;
      } else {
        slotsHtml += `<div class="time-slot" onclick="window.app.selectTimeSlot('${label}')">${label}${next}</div>`;
      }
    }
    slotsDiv.innerHTML = slotsHtml || '<p style="grid-column:1/-1;text-align:center;color:var(--gray-500);">No slots available</p>';
  }

  selectTimeSlot(time) {
    document.querySelectorAll('.time-slot').forEach(el => el.classList.remove('selected'));
    event.target.classList.add('selected');
    this.bookingState.selectedSlot = time;
    document.getElementById('confirmBookingBtn').disabled = false;
  }

  updateBookingDuration(minutes) {
    this.bookingState.duration = parseInt(minutes);
    // Recalculate total price
    const rate = this.bookingState.teacher?.hourlyRate ?? 0;
    const total = (rate * this.bookingState.duration / 60).toFixed(2);
    const totalEl = document.getElementById('bookingTotal');
    if (totalEl) totalEl.textContent = `$${total}`;
    // Refresh slots (120-min blocks an extra hour)
    this._refreshTimeSlots();
  }

  toggleRecurring(isRecurring) {
    this.bookingState.recurring = isRecurring;
  }

  confirmBooking() {
    if (!this.bookingState.selectedSlot || !this.bookingState.selectedDay) return;
    // Navigate to payment page instead of submitting immediately
    this.renderPaymentPage();
  }

  selectPaymentMethod(el) {
    document.querySelectorAll('.payment-method').forEach(item => item.classList.remove('active'));
    el.classList.add('active');
    const radio = el.querySelector('input');
    if (radio) radio.checked = true;
    this.bookingState.paymentMethod = radio?.value || 'card';
    // Show/hide the right form
    document.querySelectorAll('.payment-form-section').forEach(s => s.style.display = 'none');
    const form = document.getElementById(`payment-form-${radio?.value}`);
    if (form) form.style.display = 'block';
  }

  renderPaymentPage() {
    const bs = this.bookingState;
    const teacher = bs.teacher;
    const rate = teacher?.hourlyRate ?? 0;
    const total = (rate * bs.duration / 60).toFixed(2);
    const dateStr = bs.selectedSlot && bs.selectedDay
      ? new Date(bs.date.getFullYear(), bs.date.getMonth(), bs.selectedDay).toLocaleDateString('en-US', { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' }) + ' at ' + bs.selectedSlot
      : 'TBD';

    this.container.innerHTML = `
      <section class="booking-section">
        <div class="container" style="max-width:680px;">
          <div class="section-header">
            <h2 class="section-title">💳 Secure Payment</h2>
            <p class="section-subtitle">Complete your booking for ${teacher?.firstName ?? ''} ${teacher?.lastName ?? ''}</p>
          </div>

          <!-- Order Summary -->
          <div class="dashboard-card" style="margin-bottom:1.5rem;border-left:4px solid var(--primary);">
            <h3 style="margin-bottom:1rem;">📋 Order Summary</h3>
            <div class="booking-detail-row"><span>Teacher</span><span>${teacher?.firstName ?? ''} ${teacher?.lastName ?? ''}</span></div>
            <div class="booking-detail-row"><span>Date & Time</span><span>${dateStr}</span></div>
            <div class="booking-detail-row"><span>Duration</span><span>${bs.duration} minutes</span></div>
            <div class="booking-detail-row"><span>Rate</span><span>$${rate}/hr</span></div>
            <div class="booking-total"><span>Total</span><span style="color:var(--primary);">$${total}</span></div>
          </div>

          <!-- Stripe Payment Element -->
          <div class="dashboard-card" style="padding: 2rem;">
            <h3 style="margin-bottom:1.5rem;">🔒 Secure Payment Checkout</h3>
            <div id="payment-element">
              <div class="spinner" style="margin: 2rem auto;"></div>
            </div>
            <div id="payment-message" class="form-error" style="margin-top: 1rem; display: none;"></div>
          </div>

          <div id="paymentError" class="form-error" style="margin-top:1rem;"></div>

          <div style="display:flex;gap:1rem;margin-top:1.5rem;">
            <button class="btn btn-secondary" onclick="window.app.navigate('booking', window.app.bookingState.teacherId)" style="flex:1;">← Back</button>
            <button class="btn btn-primary btn-lg" id="payNowBtn" onclick="window.app.processPayment()" style="flex:2;">
              🔒 Pay $${total} Now
            </button>
          </div>
          <p style="text-align:center;color:var(--gray-400);font-size:0.8rem;margin-top:1rem;">🔒 Secured by Stripe & 256-bit SSL encryption</p>
        </div>
      </section>
    `;
    this.initStripePayment(parseFloat(total));
  }

  async initStripePayment(amount) {
    const bs = this.bookingState;
    try {
      const response = await api.createPaymentIntent({
        teacherId: parseInt(bs.teacherId),
        amount: amount
      });

      this.bookingState.currentPaymentId = response.paymentId;
      const clientSecret = response.clientSecret;

      const appearance = { 
        theme: 'stripe',
        variables: {
          colorPrimary: '#4f46e5',
        }
      };
      
      this.stripeElements = this.stripe.elements({ clientSecret, appearance });
      const paymentElement = this.stripeElements.create("payment");
      paymentElement.mount("#payment-element");
    } catch (e) {
      console.error("Stripe init error:", e);
      const errEl = document.getElementById('paymentError');
      if (errEl) { 
        errEl.textContent = e.message || 'Failed to connect to the payment server. Please try again.'; 
        errEl.classList.add('active'); 
      }
    }
  }

  async processPayment() {
    const errEl = document.getElementById('paymentError');
    const btn = document.getElementById('payNowBtn');
    if (btn) { btn.disabled = true; btn.innerHTML = '<div class="spinner" style="width:20px;height:20px;border-width:2px;"></div> Verifying…'; }

    try {
      const { error } = await this.stripe.confirmPayment({
        elements: this.stripeElements,
        confirmParams: {
          return_url: window.location.origin,
        },
        redirect: 'if_required' 
      });

      if (error) {
        if (errEl) { errEl.textContent = error.message; errEl.classList.add('active'); }
        if (btn) { btn.disabled = false; btn.innerHTML = '🔒 Pay Now'; }
        return;
      }

      // 1. Confirm in our database
      const bs = this.bookingState;
      await api.confirmPaymentInDb(bs.currentPaymentId);

      // 2. Create the reservation
      const [hours] = bs.selectedSlot.split(':');
      const date = new Date(bs.date);
      date.setDate(bs.selectedDay);
      date.setHours(parseInt(hours), 0, 0, 0);

      const reservationData = {
        teacherId: parseInt(bs.teacherId),
        requestedDateTime: date.toISOString(),
        durationMinutes: bs.duration,
        message: bs.recurring ? 'Recurring' : 'Lesson'
      };

      const res = await api.createReservation(reservationData);
      this.navigate('booking-confirmation', res.id);
    } catch (e) {
      console.error('Payment process error:', e);
      if (errEl) { errEl.textContent = 'Transaction failed. Please try again.'; errEl.classList.add('active'); }
      if (btn) { btn.disabled = false; btn.innerHTML = '🔒 Pay Now'; }
    }
  }

  renderBookingConfirmation(id) {
    this.container.innerHTML = `
        <section class="booking-section">
            <div class="container">
                <div class="confirmation-card">
                    <div class="success-icon">✓</div>
                    <h2>Booking Confirmed!</h2>
                    <p style="color: var(--gray-600); margin-bottom: 2rem;">
                        Your reservation request has been sent successfully. 
                        You will receive a notification once the teacher accepts.
                    </p>
                    <div class="booking-details" style="text-align: left; margin-bottom: 2rem;">
                        <div class="booking-detail-row">
                            <span>Reservation ID</span>
                            <span>#${id}</span>
                        </div>
                        <div class="booking-detail-row">
                            <span>Status</span>
                            <span class="status-badge status-pending">Pending Approval</span>
                        </div>
                    </div>
                    <button class="btn btn-primary" onclick="window.app.navigate('dashboard')">Go to Dashboard</button>
                </div>
            </div>
        </section>
    `;
  }

  async renderDashboard() {
    if (!AppState.isAuthenticated()) {
      authModule.openModal('login');
      return;
    }

    if (AppState.isAdmin()) {
      await this.renderAdmin();
    } else if (AppState.isTeacher()) {
      await this.renderTeacherDashboard();
    } else if (AppState.isParent()) {
      await this.renderParentDashboard();
    } else {
      await this.renderStudentDashboard();
    }
  }

  async renderStudentDashboard() {
    this.container.innerHTML = '<div class="loading-container"><div class="spinner"></div></div>';

    try {
      // Load initial data - if 401 is returned, the session is stale
      let lessons = [], reservations = [];
      try {
        lessons = await api.getMyLessons();
        reservations = await api.getMyReservations();
      } catch (sessionError) {
        // Session is invalid (user deleted from DB) - force logout
        authModule.logout();
        // Show a friendly notification using the notification system
        const note = document.createElement('div');
        note.style.cssText = 'position:fixed;top:90px;right:20px;background:linear-gradient(135deg,#ef4444,#dc2626);color:white;padding:1rem 1.5rem;border-radius:0.75rem;box-shadow:0 10px 25px rgba(239,68,68,0.3);z-index:3000;';
        note.textContent = 'Your session has expired. Please log in again.';
        document.body.appendChild(note);
        setTimeout(() => note.remove(), 4000);
        return;
      }

      let stats = null;
      try {
        // Find the student profile id for the current user
        const student = lessons.length > 0 ? { id: lessons[0].studentId } : (await api.getAllUsers()).find(u => u.id === AppState.currentUser.id)?.student;
        if (student) {
          stats = await api.getStudentStats(student.id);
        }
      } catch (e) {
        console.error("Failed to load stats", e);
      }

      this.dashboardData = { lessons, reservations, stats };

      this.container.innerHTML = `
        <section class="dashboard">
            <div class="container">
                <div class="dashboard-header">
                    <h1 class="dashboard-title">Student Dashboard</h1>
                </div>

                <div class="dashboard-layout">
                    <!-- Sidebar Navigation -->
                    <div class="dashboard-sidebar">
                        <div class="dashboard-nav">
                            <div class="dashboard-nav-item active" onclick="window.app.switchDashboardTab('overview')">
                                <span>📊</span> Overview
                            </div>
                            <div class="dashboard-nav-item" onclick="window.app.switchDashboardTab('lessons')">
                                <span>📅</span> My Lessons
                            </div>
                            <div class="dashboard-nav-item" onclick="window.app.switchDashboardTab('progress')">
                                <span>📈</span> Progress Tracking
                            </div>
                            <div class="dashboard-nav-item" onclick="window.app.switchDashboardTab('resources')">
                                <span>📁</span> Resources
                            </div>
                            <div class="dashboard-nav-item" onclick="window.app.switchDashboardTab('notebooks')">
                                <span>📝</span> My Notebooks
                            </div>
                            <div class="dashboard-nav-item" onclick="window.app.switchDashboardTab('messages')">
                                <span>💬</span> Messages
                            </div>
                            <div class="dashboard-nav-item" onclick="window.app.switchDashboardTab('payments')">
                                <span>💳</span> Payments & Plans
                            </div>
                        </div>
                    </div>

                    <!-- Content Area -->
                    <div class="dashboard-content-area">
                        <div id="tab-overview" class="tab-content active">
                            ${this.getDashboardOverviewHtml(lessons, reservations)}
                        </div>
                        <div id="tab-lessons" class="tab-content">
                            ${this.getDashboardLessonsHtml(lessons)}
                        </div>
                        <div id="tab-notebooks" class="tab-content">
                            ${this.getStudentNotebooksHtml(lessons)}
                        </div>
                        <div id="tab-progress" class="tab-content">
                            ${this.getDashboardProgressHtml()}
                        </div>
                        <div id="tab-resources" class="tab-content">
                            ${this.getDashboardResourcesHtml()}
                        </div>
                        <div id="tab-messages" class="tab-content">
                            <div class="loading-container"><p>Select a conversation to start messaging</p></div>
                        </div>
                        <div id="tab-payments" class="tab-content">
                            ${this.getDashboardPaymentsHtml()}
                        </div>
                    </div>
                </div>
            </div>
        </section>
      `;
    } catch (error) {
      console.error(error);
      this.container.innerHTML = '<div class="container" style="padding: 120px 0;"><p style="text-align: center; color: var(--error);">Failed to load dashboard</p></div>';
    }
  }

  async renderParentDashboard() {
    this.container.innerHTML = '<div class="loading-container"><div class="spinner"></div></div>';

    try {
      const lessons = await api.getMyLessons();
      const reservations = await api.getMyReservations();
      let children = [];
      let childrenReservations = [];
      try {
        children = await api.getMyChildren();
        childrenReservations = await api.getChildrenReservations();
      } catch (e) { /* no children yet */ }

      this.dashboardData = { lessons, reservations, children, childrenReservations };

      this.container.innerHTML = `
        <section class="dashboard">
            <div class="container">
                <div class="dashboard-header">
                    <h1 class="dashboard-title">Parent Dashboard</h1>
                    <div class="dashboard-actions">
                        <button class="btn btn-primary btn-sm" onclick="window.app.navigate('teachers')">Find New Teacher</button>
                    </div>
                </div>

                <div class="dashboard-layout">
                    <div class="dashboard-sidebar">
                        <div class="dashboard-nav">
                            <div class="dashboard-nav-item active" onclick="window.app.switchDashboardTab('overview')">
                                <span>📊</span> Overview
                            </div>
                            <div class="dashboard-nav-item" onclick="window.app.switchDashboardTab('children')">
                                <span>👥</span> My Children
                            </div>
                            <div class="dashboard-nav-item" onclick="window.app.switchDashboardTab('lessons')">
                                <span>📅</span> Schedule
                            </div>
                            <div class="dashboard-nav-item" onclick="window.app.switchDashboardTab('payments')">
                                <span>💳</span> Payments
                            </div>
                            <div class="dashboard-nav-item" onclick="window.app.switchDashboardTab('messages')">
                                <span>💬</span> Messages
                            </div>
                        </div>
                    </div>

                    <div class="dashboard-content-area">
                        <div id="tab-overview" class="tab-content active">
                            ${this.getDashboardOverviewHtml(lessons, reservations)}
                        </div>
                        <div id="tab-children" class="tab-content">
                            ${this.getParentChildrenHtml(children, childrenReservations)}
                        </div>
                        <div id="tab-lessons" class="tab-content">
                            ${this.getDashboardLessonsHtml(lessons)}
                        </div>
                        <div id="tab-payments" class="tab-content">
                            ${this.getDashboardPaymentsHtml()}
                        </div>
                        <div id="tab-messages" class="tab-content">
                            <div class="loading-container"><p>Select a conversation to start messaging</p></div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
      `;
    } catch (error) {
      console.error(error);
      this.container.innerHTML = '<div class="container" style="padding: 120px 0;"><p style="text-align: center; color: var(--error);">Failed to load dashboard</p></div>';
    }
  }

  getParentChildrenHtml(children = [], childrenReservations = []) {
    const childCards = children.length > 0 ? children.map(child => {
      const childRes = childrenReservations.filter(r => r.studentId === child.studentId);
      const initials = child.studentName ? child.studentName.split(' ').map(n => n[0]).join('') : '?';
      return `
        <div class="dashboard-card">
          <div style="display: flex; align-items: center; gap: 1rem; margin-bottom: 1rem;">
            <div class="teacher-avatar">${initials}</div>
            <div>
              <div style="font-weight: 600;">${child.studentName}</div>
              <div style="font-size: 0.85rem; color: var(--gray-500);">Level: ${child.level}</div>
            </div>
            <button class="btn btn-text btn-sm" style="margin-left:auto;color:var(--error);" onclick="window.app.unlinkChild(${child.studentId})" title="Unlink">✕</button>
          </div>
          <div class="stats-mini" style="display: grid; grid-template-columns: 1fr 1fr; gap: 1rem;">
            <div style="background: var(--gray-50); padding: 0.75rem; border-radius: 8px; text-align: center;">
              <div style="font-weight: 700; color: var(--primary);">${childRes.length}</div>
              <div style="font-size: 0.75rem; color: var(--gray-500);">Reservations</div>
            </div>
            <div style="background: var(--gray-50); padding: 0.75rem; border-radius: 8px; text-align: center;">
              <div style="font-weight: 700; color: var(--secondary);">${child.level * 100}</div>
              <div style="font-size: 0.75rem; color: var(--gray-500);">XP</div>
            </div>
          </div>
          <button class="btn btn-primary btn-sm btn-block" style="margin-top: 1rem;" onclick="window.app.navigate('teachers')">📚 Book Lesson for ${child.studentName.split(' ')[0]}</button>
        </div>`;
    }).join('') : '<div class="dashboard-card"><p style="color:var(--gray-500);text-align:center;">No children linked yet. Add a child to get started!</p></div>';

    return `
        <h2>My Children</h2>
        <div class="dashboard-grid" style="margin-top: 1.5rem;">
            ${childCards}
            <div class="dashboard-card" style="border: 2px dashed var(--gray-200); display: flex; align-items: center; justify-content: center; min-height: 150px; cursor: pointer;" onclick="window.app.showLinkChildModal()">
                <div style="text-align: center; color: var(--gray-500);">
                    <div style="font-size: 2rem;">➕</div>
                    <div style="margin-top: 0.5rem; font-weight: 500;">Link Child Account</div>
                    <div style="font-size: 0.8rem; margin-top: 0.25rem;">by email address</div>
                </div>
            </div>
        </div>
    `;
  }

  async showLinkChildModal() {
    const email = prompt('Enter your child\'s account email address:');
    if (!email) return;
    try {
      const result = await api.linkChild(email);
      alert(result.message || 'Child linked successfully!');
      this.renderParentDashboard();
    } catch (err) {
      alert(err.message || 'Failed to link child');
    }
  }

  async unlinkChild(studentId) {
    if (!confirm('Are you sure you want to unlink this child?')) return;
    try {
      await api.unlinkChild(studentId);
      this.renderParentDashboard();
    } catch (err) {
      alert(err.message || 'Failed to unlink child');
    }
  }

  async renderTeacherDashboard() {
    this.container.innerHTML = '<div class="loading-container"><div class="spinner"></div></div>';

    try {
      const lessons = await api.getMyLessons();
      const reservations = await api.getMyReservations(); // Pending requests

      let schedule = [];
      try {
        schedule = await api.getMySchedule();
      } catch (e) { console.warn("Could not fetch schedule", e); }

      this.dashboardData = { lessons, reservations, schedule };

      this.container.innerHTML = `
        <section class="dashboard">
            <div class="container">
                <div class="dashboard-header">
                    <h1 class="dashboard-title">Teacher Dashboard</h1>
                    <div class="dashboard-actions">
                        <button class="btn btn-primary btn-sm" onclick="window.app.switchDashboardTab('schedule')">Update Availability</button>
                    </div>
                </div>

                <div class="dashboard-layout">
                    <!-- Sidebar Navigation -->
                    <div class="dashboard-sidebar">
                        <div class="dashboard-nav">
                            <div class="dashboard-nav-item active" onclick="window.app.switchDashboardTab('overview')">
                                <span>📊</span> Overview
                            </div>
                            <div class="dashboard-nav-item" onclick="window.app.switchDashboardTab('profile')">
                                <span>👤</span> Profile
                            </div>
                            <div class="dashboard-nav-item" onclick="window.app.switchDashboardTab('schedule')">
                                <span>📅</span> Schedule
                            </div>
                            <div class="dashboard-nav-item" onclick="window.app.switchDashboardTab('lessons')">
                                <span>🎓</span> Lessons
                            </div>
                            <div class="dashboard-nav-item" onclick="window.app.switchDashboardTab('students')">
                                <span>👥</span> Students
                            </div>
                            <div class="dashboard-nav-item" onclick="window.app.switchDashboardTab('messages')">
                                <span>💬</span> Messages
                            </div>
                        </div>
                    </div>

                    <!-- Content Area -->
                    <div class="dashboard-content-area">
                        <div id="tab-overview" class="tab-content active">
                            ${this.getTeacherOverviewHtml(lessons, reservations)}
                        </div>
                        <div id="tab-profile" class="tab-content">
                            ${this.getTeacherProfileHtml()}
                        </div>
                        <div id="tab-schedule" class="tab-content">
                            ${this.getTeacherScheduleHtml()}
                        </div>
                        <div id="tab-lessons" class="tab-content">
                            ${this.getTeacherLessonsHtml(lessons)}
                        </div>
                         <div id="tab-students" class="tab-content">
                            ${this.getTeacherStudentsHtml(lessons)}
                        </div>
                        <div id="tab-messages" class="tab-content">
                            <div class="loading-container"><p>Select a conversation to start messaging</p></div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
      `;
    } catch (error) {
      console.error(error);
      this.container.innerHTML = '<div class="container" style="padding: 120px 0;"><p style="text-align: center; color: var(--error);">Failed to load dashboard</p></div>';
    }
  }

  switchDashboardTab(tabName) {
    // Update nav items
    document.querySelectorAll('.dashboard-nav-item').forEach(item => {
      if (item.innerText.toLowerCase().includes(tabName)) {
        item.classList.add('active');
      } else {
        item.classList.remove('active');
      }
    });

    // Update content
    document.querySelectorAll('.tab-content').forEach(content => {
      content.classList.remove('active');
    });
    const targetTab = document.getElementById(`tab-${tabName}`);
    if (targetTab) targetTab.classList.add('active');

    if (tabName === 'messages') {
      this.loadDashboardMessages();
    }
  }

  getTeacherOverviewHtml(lessons, reservations) {
    const upcoming = lessons.filter(l => new Date(l.scheduledDateTime) > new Date()).slice(0, 5);
    const pending = reservations.filter(r => r.status === 'Pending').slice(0, 5);

    return `
        <h2>Teacher Overview</h2>
        <div class="stats-grid" style="margin-top: 1.5rem; margin-bottom: 2rem;">
            <div class="stat-card">
                <div class="stat-number">${lessons.length}</div>
                <div class="stat-label">Total Lessons</div>
            </div>
            <div class="stat-card">
                <div class="stat-number">${new Set(lessons.map(l => l.studentId)).size}</div>
                <div class="stat-label">Active Students</div>
            </div>
            <div class="stat-card">
                <div class="stat-number">${pending.length}</div>
                <div class="stat-label">Pending Requests</div>
            </div>
            <div class="stat-card">
                <div class="stat-number">$${(lessons.length * 40).toFixed(0)}</div>
                <div class="stat-label">Total Earnings</div>
            </div>
        </div>

        <div class="dashboard-grid">
            <div class="dashboard-card">
                <h3>Upcoming Lessons</h3>
                <div class="lesson-list">
                    ${upcoming.length ? upcoming.map(l => this.createLessonItem(l)).join('') : '<p>No upcoming lessons</p>'}
                </div>
            </div>
            <div class="dashboard-card">
                <h3>New Requests</h3>
                <div class="lesson-list">
                    ${pending.length ? pending.map(r => `
                        <div class="lesson-item">
                            <div class="lesson-title">${r.studentName}</div>
                            <div class="lesson-meta">
                                ${new Date(r.requestedDateTime).toLocaleString()}
                                <div style="margin-top: 0.5rem; display: flex; gap: 0.5rem;">
                                    <button class="btn btn-primary btn-sm" onclick="window.app.updateReservation(${r.id}, 'Confirmed')">Accept</button>
                                    <button class="btn btn-secondary btn-sm" onclick="window.app.updateReservation(${r.id}, 'Cancelled')">Decline</button>
                                </div>
                            </div>
                        </div>
                    `).join('') : '<p>No pending requests</p>'}
                </div>
            </div>
        </div>
    `;
  }

  async submitTeacherVerification() {
    const input = document.getElementById('certUrlInput');
    if (!input || !input.value) {
      alert('Please provide a valid certificate URL');
      return;
    }

    try {
      await api.submitVerification(input.value);
      alert('Verification request submitted successfully!');
      // Update local state and reload tab
      AppState.currentUser.verificationStatus = 'Pending';
      this.switchDashboardTab('profile');
    } catch (err) {
      alert(err.message || 'Failed to submit verification');
    }
  }

  getTeacherProfileHtml() {
    const user = AppState.currentUser;
    const verificationStatus = user.verificationStatus || 'Unknown';
    const isVerified = verificationStatus === 'Verified';
    const isPending = verificationStatus === 'Pending';

    return `
        <h2>Profile Management</h2>

        <!-- Verification Section -->
        <div class="dashboard-card" style="margin-top: 1.5rem; border-left: 4px solid ${isVerified ? 'var(--success)' : isPending ? 'var(--warning)' : 'var(--error)'};">
            <div style="display:flex; justify-content:space-between; align-items:center;">
                <div>
                    <h3 style="margin-bottom:0.5rem;">Teacher Verification</h3>
                    <p style="font-size:0.9rem; color:var(--gray-600);">
                        ${isVerified ? '✅ Your account is verified. You are visible in the search catalog.' :
        isPending ? '⏳ Your verification is currently being reviewed by an admin.' :
          '❌ Your account is not verified. You will not appear in search results until verified.'}
                    </p>
                </div>
                <span class="status-badge ${isVerified ? 'status-confirmed' : isPending ? 'status-pending' : 'status-cancelled'}" style="font-size:0.9rem; padding: 0.5rem 1rem;">
                    ${verificationStatus}
                </span>
            </div>
            
            ${!isVerified && !isPending ? `
                <div style="margin-top:1.5rem; padding-top:1.5rem; border-top:1px solid var(--gray-100);">
                    <p style="font-size:0.85rem; margin-bottom:1rem; color:var(--gray-500);">To verify your account, please provide a link to your teaching certificate or diploma (PDF or Image URL):</p>
                    <div style="display:flex; gap:0.5rem;">
                        <input type="url" id="certUrlInput" class="form-input" placeholder="https://example.com/my-certificate.pdf" style="flex:1;">
                        <button class="btn btn-primary btn-sm" onclick="window.app.submitTeacherVerification()">Submit for Review</button>
                    </div>
                </div>
            ` : ''}

            ${user.verificationNotes ? `
                <div style="margin-top:1rem; font-size:0.85rem; padding:0.75rem; background:var(--gray-50); border-radius:8px;">
                    <strong>Admin Note:</strong> ${user.verificationNotes}
                </div>
            ` : ''}
        </div>

        <div class="dashboard-card" style="margin-top: 1.5rem;">
            <form id="teacherProfileForm" onsubmit="event.preventDefault(); alert('Profile updated!')">
                <div class="form-row">
                    <div class="form-group">
                        <label>First Name</label>
                        <input type="text" class="form-input" value="${user.firstName}" required>
                    </div>
                    <div class="form-group">
                        <label>Last Name</label>
                        <input type="text" class="form-input" value="${user.lastName}" required>
                    </div>
                </div>
                <div class="form-group">
                    <label>Bio</label>
                    <textarea class="form-input" rows="4">${user.bio || 'Professional educator.'}</textarea>
                </div>
                <div class="form-row">
                    <div class="form-group">
                        <label>Hourly Rate ($)</label>
                        <input type="number" class="form-input" value="40">
                    </div>
                    <div class="form-group">
                        <label>Subjects (comma separated)</label>
                        <input type="text" class="form-input" value="Mathematics, Physics">
                    </div>
                </div>
                <button type="submit" class="btn btn-primary">Save Changes</button>
            </form>
        </div>
    `;
  }

  getTeacherScheduleHtml() {
    const days = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];
    const schedules = this.dashboardData?.schedule || [];

    return `
        <h2>My Schedule</h2>
        <div class="dashboard-card" style="margin-top: 1.5rem;">
            <p>Set your weekly availability slots.</p>
            <div class="schedule-grid" style="display: grid; gap: 1rem; margin-top: 1rem;">
                ${days.map(day => {
      const s = schedules.find(x => x.dayOfWeek === day);
      const status = s?.isAvailable ? `<span class="status-badge status-confirmed">${s.startTime} - ${s.endTime}</span>` : `<span class="status-badge status-cancelled">Not Available</span>`;
      return `
                    <div style="display: flex; align-items: center; justify-content: space-between; padding: 1rem; background: var(--gray-50); border-radius: 8px;">
                        <span style="font-weight: 600;">${day}</span>
                        <div style="display: flex; align-items: center; gap: 0.5rem;">
                            ${status}
                            <button class="btn btn-text btn-sm" onclick="window.app.editSchedule('${day}')">Edit</button>
                        </div>
                    </div>`;
    }).join('')}
            </div>
            <button class="btn btn-primary" onclick="window.app.saveSchedule()" style="margin-top:1.5rem;">Save Changes</button>
        </div>

        <!-- Schedule Edit Modal inside the tab -->
        <div class="modal" id="scheduleModal">
            <div class="modal-overlay" onclick="document.getElementById('scheduleModal').classList.remove('active')"></div>
            <div class="modal-content" style="padding: 1.5rem;">
                <h3 style="margin-bottom:1rem;" id="scheduleModalTitle">Edit Schedule</h3>
                <input type="hidden" id="editScheduleDay" />
                <div class="form-group">
                    <label style="display:flex; align-items:center; gap:0.5rem; cursor:pointer;">
                        <input type="checkbox" id="editScheduleAvailable" onchange="document.getElementById('editScheduleTimes').style.display=this.checked?'block':'none'">
                        <span>Available on this day</span>
                    </label>
                </div>
                <div id="editScheduleTimes">
                    <div class="form-row">
                        <div class="form-group"><label>Start Time</label><input type="time" class="form-input" id="editScheduleStart"></div>
                        <div class="form-group"><label>End Time</label><input type="time" class="form-input" id="editScheduleEnd"></div>
                    </div>
                </div>
                <div style="display:flex; gap:1rem; margin-top:1.5rem;">
                    <button class="btn btn-secondary" style="flex:1;" onclick="document.getElementById('scheduleModal').classList.remove('active')">Cancel</button>
                    <button class="btn btn-primary" style="flex:1;" onclick="window.app.applyScheduleEdit()">Apply</button>
                </div>
            </div>
        </div>
    `;
  }

  editSchedule(day) {
    const schedules = this.dashboardData?.schedule || [];
    const s = schedules.find(x => x.dayOfWeek === day);

    document.getElementById('scheduleModalTitle').textContent = `Edit ${day}`;
    document.getElementById('editScheduleDay').value = day;

    document.getElementById('editScheduleAvailable').checked = s?.isAvailable ?? false;
    document.getElementById('editScheduleStart').value = s?.startTime || '09:00';
    document.getElementById('editScheduleEnd').value = s?.endTime || '17:00';
    document.getElementById('editScheduleTimes').style.display = (s?.isAvailable ?? false) ? 'block' : 'none';

    document.getElementById('scheduleModal').classList.add('active');
  }

  applyScheduleEdit() {
    const day = document.getElementById('editScheduleDay').value;
    const isAvail = document.getElementById('editScheduleAvailable').checked;
    const start = document.getElementById('editScheduleStart').value;
    const end = document.getElementById('editScheduleEnd').value;

    let schedules = this.dashboardData.schedule;
    if (!schedules) this.dashboardData.schedule = schedules = [];

    const existingIndex = schedules.findIndex(x => x.dayOfWeek === day);
    const newEntry = { dayOfWeek: day, startTime: start, endTime: end, isAvailable: isAvail };

    if (existingIndex >= 0) schedules[existingIndex] = newEntry;
    else schedules.push(newEntry);

    document.getElementById('scheduleModal').classList.remove('active');

    // Re-render only schedule tab
    document.getElementById('tab-schedule').innerHTML = this.getTeacherScheduleHtml();
  }

  async saveSchedule() {
    try {
      if (!this.dashboardData || !this.dashboardData.schedule) return;
      await api.updateTeacherSchedule(this.dashboardData.schedule);
      alert('Schedule updated successfully!');
    } catch (err) {
      alert(err.message || 'Failed to update schedule');
    }
  }

  getTeacherLessonsHtml(lessons) {
    const upcoming = lessons.filter(l => new Date(l.scheduledDateTime) > new Date());
    const past = lessons.filter(l => new Date(l.scheduledDateTime) <= new Date());

    return `
        <h2>Lesson Management</h2>
        <div style="margin-top: 1.5rem;">
            <h3 style="margin-bottom: 1rem;">Upcoming</h3>
            <div class="lesson-list">
                ${upcoming.length ? upcoming.map(l => this.createLessonItem(l)).join('') : '<p>No upcoming lessons</p>'}
            </div>
            <h3 style="margin-top: 2rem; margin-bottom: 1rem;">Past Lessons</h3>
            <div class="lesson-list">
                ${past.length ? past.map(l => this.createLessonItem(l)).join('') : '<p>No past lessons</p>'}
            </div>
        </div>
    `;
  }

  getTeacherStudentsHtml(lessons) {
    const students = [...new Set(lessons.map(l => ({ id: l.studentId, name: l.studentName })))];
    return `
        <h2>My Students</h2>
        <div class="dashboard-grid" style="margin-top: 1.5rem;">
            ${students.length ? students.map(s => `
                <div class="dashboard-card">
                    <div style="display: flex; align-items: center; gap: 1rem;">
                        <div class="teacher-avatar">${s.name[0]}</div>
                        <div>
                            <div style="font-weight: 600;">${s.name}</div>
                            <div style="font-size: 0.85rem; color: var(--gray-500);">Student Profile</div>
                        </div>
                    </div>
                    <div style="margin-top: 1rem; display: flex; gap: 0.5rem;">
                        <button class="btn btn-secondary btn-sm" onclick="window.app.navigate('messages', ${s.id})">Message</button>
                        <button class="btn btn-text btn-sm" onclick="window.app.navigate('notebook', 'student-${s.id}')">Notebook</button>
                    </div>
                </div>
            `).join('') : '<p>No students yet</p>'}
        </div>
    `;
  }

  getStudentNotebooksHtml(lessons) {
    const teachers = [...new Map(lessons.map(l => [l.teacherId, { id: l.teacherId, name: l.teacherName }])).values()];
    return `
        <h2>My Notebooks</h2>
        <p style="color: var(--gray-600); margin-bottom: 1.5rem;">Access your shared notes with each of your teachers.</p>
        <div class="dashboard-grid">
            ${teachers.length ? teachers.map(t => `
                <div class="dashboard-card" style="border-left: 4px solid var(--secondary);">
                    <div style="display: flex; align-items: center; gap: 1rem; margin-bottom: 1rem;">
                        <div class="teacher-avatar" style="background: var(--gray-100); color: var(--secondary); font-size: 1.2rem;">👨‍🏫</div>
                        <div>
                            <div style="font-weight: 600;">Notes with ${t.name}</div>
                            <div style="font-size: 0.85rem; color: var(--gray-500);">Last edited: Recently</div>
                        </div>
                    </div>
                    <button class="btn btn-primary btn-sm btn-block" onclick="window.app.navigate('notebook', 'teacher-${t.id}')">
                        <span style="margin-right:0.5rem;">📝</span> Open Notebook
                    </button>
                </div>
            `).join('') : '<div class="dashboard-card"><p style="text-align: center; color: var(--gray-500);">No notebooks found. Start a lesson to begin taking notes!</p></div>'}
        </div>
    `;
  }

  getDashboardOverviewHtml(lessons, reservations) {
    const combinedUpcoming = [
      ...lessons.filter(l => new Date(l.scheduledDateTime) > new Date()).map(l => ({ ...l, isReservation: false })),
      ...reservations.filter(r => r.status === 'Pending').map(r => ({ ...r, isReservation: true }))
    ].sort((a, b) => {
      const dateA = new Date(a.isReservation ? a.requestedDateTime : a.scheduledDateTime);
      const dateB = new Date(b.isReservation ? b.requestedDateTime : b.scheduledDateTime);
      return dateA - dateB;
    });

    const pending = reservations.filter(r => r.status === 'Pending').slice(0, 3);
    const stats = this.dashboardData?.stats || {
      experiencePoints: 0,
      level: 1,
      currentStreak: 0,
      nextLevelXp: 500
    };

    const progressPercent = (stats.experiencePoints / stats.nextLevelXp) * 100;

    return `
        <div class="dashboard-welcome" style="margin-bottom: 2rem;">
            <h2 style="font-size: 1.5rem; margin-bottom: 0.5rem;">Welcome back, ${AppState.currentUser.firstName}! 👋</h2>
            <p style="color: var(--gray-600);">Here's what's happening with your learning journey.</p>
        </div>

        <!-- Quick Stats Row -->
        <div class="stats-grid" style="margin-bottom: 2.5rem; display: grid; grid-template-columns: repeat(auto-fit, minmax(180px, 1fr)); gap: 1.25rem;">
            <div class="stat-card" style="background: white; padding: 1.5rem; border-radius: 16px; box-shadow: var(--shadow-sm); text-align: left; border-left: 4px solid var(--primary);">
                <div style="font-size: 0.875rem; color: var(--gray-500); font-weight: 500;">Upcoming Classes</div>
                <div style="font-size: 1.75rem; font-weight: 800; color: var(--gray-900); margin: 0.5rem 0;">${combinedUpcoming.length}</div>
                <div style="font-size: 0.75rem; color: var(--success); font-weight: 600;">Next: ${combinedUpcoming[0] ? new Date(combinedUpcoming[0].isReservation ? combinedUpcoming[0].requestedDateTime : combinedUpcoming[0].scheduledDateTime).toLocaleDateString() : 'N/A'}</div>
            </div>
            <div class="stat-card" style="background: white; padding: 1.5rem; border-radius: 16px; box-shadow: var(--shadow-sm); text-align: left; border-left: 4px solid var(--success);">
                <div style="font-size: 0.875rem; color: var(--gray-500); font-weight: 500;">Completed Classes</div>
                <div style="font-size: 1.75rem; font-weight: 800; color: var(--gray-900); margin: 0.5rem 0;">${lessons.filter(l => new Date(l.scheduledDateTime) <= new Date()).length}</div>
                <div style="font-size: 0.75rem; color: var(--gray-500);">Total learning time</div>
            </div>
            <div class="stat-card" style="background: white; padding: 1.5rem; border-radius: 16px; box-shadow: var(--shadow-sm); text-align: left; border-left: 4px solid var(--accent);">
                <div style="font-size: 0.875rem; color: var(--gray-500); font-weight: 500;">Total XP Points</div>
                <div style="font-size: 1.75rem; font-weight: 800; color: var(--gray-900); margin: 0.5rem 0;">${stats.experiencePoints}</div>
                <div style="font-size: 0.75rem; color: var(--gray-500);">Keep it up!</div>
            </div>
            <div class="stat-card" style="background: white; padding: 1.5rem; border-radius: 16px; box-shadow: var(--shadow-sm); text-align: left; border-left: 4px solid var(--warning);">
                <div style="font-size: 0.875rem; color: var(--gray-500); font-weight: 500;">Daily Streak</div>
                <div style="font-size: 1.75rem; font-weight: 800; color: var(--gray-900); margin: 0.5rem 0;">${stats.currentStreak} 🔥</div>
                <div style="font-size: 0.75rem; color: var(--gray-500);">Don't break the streak!</div>
            </div>
        </div>

        <div class="dashboard-grid" style="display: grid; grid-template-columns: 1fr 1fr; gap: 2rem;">
            <div class="dashboard-column">
                <div class="dashboard-card" style="height: 100%;">
                    <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem;">
                        <h3 style="font-size: 1.1rem; font-weight: 700;">📅 Upcoming Lessons</h3>
                        <button class="btn btn-text btn-sm" onclick="window.app.switchDashboardTab('lessons')">View All</button>
                    </div>
                    <div class="lesson-list">
                        ${combinedUpcoming.length ? combinedUpcoming.slice(0, 5).map(item => this.createLessonItem(item, item.isReservation)).join('') : `
                            <div style="text-align: center; padding: 2rem; background: var(--gray-50); border-radius: 12px;">
                                <div style="font-size: 2rem; margin-bottom: 0.5rem;">🎓</div>
                                <p style="color: var(--gray-500);">No upcoming lessons scheduled.</p>
                                <button class="btn btn-primary btn-sm" style="margin-top: 1rem;" onclick="window.app.navigate('teachers')">Find a Teacher</button>
                            </div>
                        `}
                    </div>
                </div>
            </div>

            <div class="dashboard-column">
                <div class="dashboard-card" style="margin-bottom: 1.5rem;">
                    <h3 style="font-size: 1.1rem; font-weight: 700; margin-bottom: 1.5rem;">📈 Learning Progress</h3>
                    <div class="progress-info">
                        <div style="display: flex; justify-content: space-between; margin-bottom: 0.75rem; font-weight: 600;">
                            <span>Current Level: ${stats.level}</span>
                            <span style="color: var(--primary);">${Math.round(progressPercent)}% to Lvl ${stats.level + 1}</span>
                        </div>
                        <div class="xp-progress-container" style="background: var(--gray-100); height: 12px; border-radius: 6px; overflow: hidden;">
                            <div class="xp-progress-bar" style="width: ${progressPercent}%; height: 100%; background: linear-gradient(90deg, var(--primary), var(--secondary));"></div>
                        </div>
                        <p style="font-size: 0.85rem; color: var(--gray-500); margin-top: 0.5rem;">
                            You need ${stats.nextLevelXp - stats.experiencePoints} more XP to reach the next level.
                        </p>
                    </div>
                    <button class="btn btn-secondary btn-block btn-sm" style="margin-top: 1.5rem;" onclick="window.app.switchDashboardTab('progress')">Detailed Progress</button>
                </div>

                <div class="dashboard-card">
                    <h3 style="font-size: 1.1rem; font-weight: 700; margin-bottom: 1rem;">⏳ Pending Requests</h3>
                    <div class="lesson-list">
                        ${pending.length ? pending.map(r => this.createLessonItem(r, true)).join('') : '<p style="color:var(--gray-500); font-size: 0.9rem;">No pending lesson requests.</p>'}
                    </div>
                </div>
            </div>
        </div>
    `;
  }

  getDashboardLessonsHtml(lessons) {
    const upcoming = lessons.filter(l => new Date(l.scheduledDateTime) > new Date());
    const past = lessons.filter(l => new Date(l.scheduledDateTime) <= new Date());
    const pending = this.dashboardData?.reservations?.filter(r => r.status === 'Pending') || [];

    return `
        <h2>My Lessons</h2>
        <div style="margin-top: 1.5rem;">
            ${pending.length > 0 ? `
              <h3 style="margin-bottom: 1rem; color: var(--warning);">Pending Approval</h3>
              <div class="lesson-list" style="margin-bottom: 2rem;">
                  ${pending.map(r => this.createLessonItem(r, true)).join('')}
              </div>
            ` : ''}

            <h3 style="margin-bottom: 1rem;">Upcoming Confirmed</h3>
            <div class="lesson-list" style="margin-bottom: 2rem;">
                 ${upcoming.length ? upcoming.map(l => this.createLessonItem(l)).join('') : '<p>No confirmed upcoming lessons</p>'}
            </div>

            <h3 style="margin-bottom: 1rem;">Past History</h3>
             <div class="lesson-list">
                 ${past.length ? past.map(l => this.createLessonItem(l)).join('') : '<p>No past lessons</p>'}
            </div>
        </div>
    `;
  }

  getDashboardProgressHtml() {
    const stats = this.dashboardData?.stats || {
      experiencePoints: 0,
      level: 1,
      currentStreak: 0,
      nextLevelXp: 500,
      badges: []
    };

    const progressPercent = (stats.experiencePoints / stats.nextLevelXp) * 100;

    return `
        <h2>Progress Tracking</h2>
        
        <div class="progress-section">
            <div style="display:flex; justify-content:space-between; align-items:center;">
                <h3>Level ${stats.level} Scholar</h3>
                <div class="streak-counter">🔥 ${stats.currentStreak} Day Streak</div>
            </div>
            <div class="level-progress">
                <div class="xp-progress-container">
                    <div class="xp-progress-bar" style="width: ${progressPercent}%"></div>
                </div>
                <div style="display: flex; justify-content: space-between; font-size: 0.85rem; color: var(--gray-500); margin-top: 0.5rem;">
                    <span>${stats.experiencePoints} XP</span>
                    <span>${stats.nextLevelXp} XP to Level ${stats.level + 1}</span>
                </div>
            </div>
            
            <div class="badge-grid">
                ${stats.badges.length ? stats.badges.map(b => `
                    <div class="badge-item" title="${b.description}">
                        <span class="badge-icon">${b.icon}</span>
                        <span class="badge-name">${b.name}</span>
                    </div>
                `).join('') : '<p style="color:var(--gray-500)">No badges earned yet. Complete lessons to earn badges!</p>'}
            </div>
        </div>

        <!-- Weekly Report Mockup Integration -->
        <div class="progress-section" style="margin-top: 2rem;">
            <h3>Instructor Feedback & Weekly Reports</h3>
            <div class="feedback-list">
                <div class="feedback-item">
                    <div class="feedback-header">
                        <span class="feedback-date">Auto-Generated Report</span>
                        <span class="feedback-author">Platform System</span>
                    </div>
                    <p class="feedback-text">Your weekly summary is ready. You completed ${this.dashboardData.lessons.length} lessons this month!</p>
                    <button class="btn btn-secondary btn-sm" onclick="alert('Downloading Weekly Report PDF...')">View Weekly Report (PDF)</button>
                </div>
            </div>
        </div>
    `;
  }

  getDashboardResourcesHtml() {
    return `
        <h2>Resources & Materials Library</h2>
        
        <div class="resources-tabs" style="display: flex; gap: 1rem; margin-bottom: 1.5rem; border-bottom: 1px solid var(--gray-200); padding-bottom: 0.5rem;">
            <button class="btn btn-text active">All</button>
            <button class="btn btn-text">PDFs</button>
            <button class="btn btn-text">Videos</button>
            <button class="btn btn-text">Exercises</button>
        </div>

        <div class="assignment-list">
             <div class="assignment-item">
                <div style="display: flex; gap: 1rem; align-items: center;">
                    <div class="file-icon">🎥</div>
                    <div>
                        <div style="font-weight: 600;">Calculus Fundamentals - Video Lesson</div>
                        <div style="font-size: 0.875rem; color: var(--gray-500);">Video Lesson • 45 min</div>
                    </div>
                </div>
                <button class="btn btn-secondary btn-sm">Watch</button>
            </div>
            
            <div class="assignment-item">
                <div style="display: flex; gap: 1rem; align-items: center;">
                    <div class="file-icon">📝</div>
                    <div>
                        <div style="font-weight: 600;">Calculus Homework 1.pdf</div>
                        <div style="font-size: 0.875rem; color: var(--gray-500);">Worksheet • Dec 8, 2024</div>
                    </div>
                </div>
                <button class="btn btn-secondary btn-sm">Download</button>
            </div>
            
             <div class="assignment-item">
                <div style="display: flex; gap: 1rem; align-items: center;">
                    <div class="file-icon">⚡</div>
                    <div>
                        <div style="font-weight: 600;">Physics Practice Quiz</div>
                        <div style="font-size: 0.875rem; color: var(--gray-500);">Interactive Exercise</div>
                    </div>
                </div>
                <button class="btn btn-secondary btn-sm">Start</button>
            </div>

            <div class="assignment-item">
                <div style="display: flex; gap: 1rem; align-items: center;">
                    <div class="file-icon">📂</div>
                    <div>
                        <div style="font-weight: 600;">Teacher Uploads Folder</div>
                        <div style="font-size: 0.875rem; color: var(--gray-500);">Shared Resources</div>
                    </div>
                </div>
                <button class="btn btn-secondary btn-sm">Open</button>
            </div>
        </div>
    `;
  }

  getDashboardPaymentsHtml() {
    return `
        <h2>Payments & Plans</h2>
        
        <!-- Plans -->
         <div class="plans-grid" style="display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 1.5rem; margin-bottom: 3rem;">
            <div class="plan-card" style="border: 1px solid var(--gray-200); padding: 1.5rem; border-radius: 12px; text-align: center;">
                <h3>Single Lesson</h3>
                <div class="plan-price" style="font-size: 1.5rem; font-weight: 700; margin: 1rem 0;">Pay as you go</div>
                <p style="color: var(--gray-500); margin-bottom: 1.5rem;">Flexible booking</p>
                <button class="btn btn-secondary btn-block">Book Now</button>
            </div>
            
            <div class="plan-card featured" style="border: 2px solid var(--primary); padding: 1.5rem; border-radius: 12px; text-align: center; position: relative;">
                 <div style="position: absolute; top: -10px; left: 50%; transform: translateX(-50%); background: var(--primary); color: white; padding: 2px 10px; border-radius: 10px; font-size: 0.75rem;">POPULAR</div>
                <h3>Monthly Sub</h3>
                <div class="plan-price" style="font-size: 1.5rem; font-weight: 700; margin: 1rem 0;">$199<span style="font-size: 1rem; font-weight: 400;">/mo</span></div>
                <p style="color: var(--gray-500); margin-bottom: 1.5rem;">4 Lessons + Materials</p>
                <button class="btn btn-primary btn-block">Subscribe</button>
            </div>
            
             <div class="plan-card" style="border: 1px solid var(--gray-200); padding: 1.5rem; border-radius: 12px; text-align: center;">
                <h3>10 Lesson Package</h3>
                <div class="plan-price" style="font-size: 1.5rem; font-weight: 700; margin: 1rem 0;">$450</div>
                <p style="color: var(--gray-500); margin-bottom: 1.5rem;">Save 10%</p>
                <button class="btn btn-secondary btn-block">Purchase</button>
            </div>
        </div>

        <h3>Payment History & Invoices</h3>
        <div style="margin-top: 1.5rem; overflow-x: auto;">
            <table class="payment-table">
                <thead>
                    <tr>
                        <th>Date</th>
                        <th>Description</th>
                        <th>Amount</th>
                        <th>Status</th>
                        <th>Invoice</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>Dec 8, 2024</td>
                        <td>Physics Lesson (60 min)</td>
                        <td>$45.00</td>
                        <td><span class="status-badge status-confirmed">Paid</span></td>
                        <td><a href="#" class="invoice-btn">Download PDF</a></td>
                    </tr>
                     <tr>
                        <td>Dec 5, 2024</td>
                        <td>Monthly Subscription (Dec)</td>
                        <td>$199.00</td>
                        <td><span class="status-badge status-confirmed">Paid</span></td>
                        <td><a href="#" class="invoice-btn">Download PDF</a></td>
                    </tr>
                    <tr>
                        <td>Dec 1, 2024</td>
                        <td>Math Lesson (60 min)</td>
                        <td>$40.00</td>
                        <td><span class="status-badge status-confirmed">Paid</span></td>
                        <td><a href="#" class="invoice-btn">Download PDF</a></td>
                    </tr>
                </tbody>
            </table>
        </div>
    `;
  }

  async loadDashboardMessages() {
    const container = document.getElementById('tab-messages');
    try {
      const conversations = await api.getConversations();
      container.innerHTML = `
            <h2>Messages</h2>
            <div class="dashboard-grid" style="grid-template-columns: 1fr; margin-top: 1.5rem;">
                ${conversations.map(c => `
                    <div class="lesson-item" style="cursor: pointer;" onclick="window.app.navigate('messages', ${c.otherUserId})">
                        <div class="lesson-title">${c.otherUserName}</div>
                        <div class="lesson-meta">${c.lastMessageContent}</div>
                        <div class="lesson-meta" style="margin-top: 0.25rem; font-size: 0.75rem;">${new Date(c.lastMessageDate).toLocaleString()}</div>
                    </div>
                `).join('')}
            </div>
        `;
    } catch (e) {
      container.innerHTML = '<p>No messages yet.</p>';
    }
  }

  createLessonItem(item, isReservation = false) {
    const name = AppState.isStudentOrParent() ? item.teacherName : item.studentName;
    const date = new Date(isReservation ? item.requestedDateTime : item.scheduledDateTime).toLocaleString();

    return `
        <div class="lesson-item">
            <div class="lesson-title">${name}</div>
            <div class="lesson-meta">
                ${date}
                ${isReservation ? `<span class="status-badge status-pending">${item.status}</span>
                  <div style="margin-top: 0.5rem; display: flex; gap: 0.5rem;">
                    <button class="btn btn-primary btn-sm" onclick="event.stopPropagation(); window.app.navigate('videocall', '${item.id}')" style="background:linear-gradient(135deg,#06b6d4,#3b82f6);">📹 Video Call</button>
                  </div>
                ` : `
                    <div style="margin-top: 0.5rem; display: flex; gap: 0.5rem; flex-wrap: wrap;">
                        <button class="btn btn-primary btn-sm" onclick="event.stopPropagation(); window.app.navigate('videocall', '${item.id}')" style="background:linear-gradient(135deg,#06b6d4,#3b82f6);">📹 Video Call</button>
                        <button class="btn btn-secondary btn-sm" onclick="event.stopPropagation(); window.app.navigate('whiteboard', '${item.id}')">🎨 Whiteboard</button>
                        <button class="btn btn-secondary btn-sm" onclick="event.stopPropagation(); window.app.navigate('notebook', '${item.id}')">📝 Notebook</button>
                    </div>
                `}
            </div>
        </div>
    `;
  }

  async updateReservation(id, status) {
    try {
      await api.updateReservationStatus(id, status);
      authModule.showSuccess(`Reservation ${status.toLowerCase()} successfully!`);
      this.renderDashboard();
    } catch (error) {
      console.error('Update Reservation Error:', error);
      alert('Failed to update reservation: ' + error.message);
    }
  }

  async renderAdmin() {
    if (!AppState.isAdmin()) {
      this.navigate('home');
      return;
    }

    this.container.innerHTML = '<div class="loading-container"><div class="spinner"></div></div>';

    try {
      const stats = await api.getDashboardStats();
      const users = await api.getAllUsers();
      const lessons = await api.getAllLessons();
      let pendingVerifications = [];
      try { pendingVerifications = await api.getVerifications('Pending'); } catch (e) { }

      this.container.innerHTML = `
        <section class="dashboard">
          <div class="container">
            <div class="dashboard-header">
              <h1 class="dashboard-title">Admin Dashboard</h1>
              <p style="color: var(--gray-600);">Platform overview and management</p>
            </div>

            <div class="stats-grid" style="margin-bottom: 2rem;">
              <div class="stat-card">
                <div class="stat-number">${stats.totalStudents}</div>
                <div class="stat-label">Total Students</div>
              </div>
              <div class="stat-card">
                <div class="stat-number">${stats.totalTeachers}</div>
                <div class="stat-label">Total Teachers</div>
              </div>
              <div class="stat-card">
                <div class="stat-number">${stats.totalLessons}</div>
                <div class="stat-label">Total Lessons</div>
              </div>
              <div class="stat-card">
                <div class="stat-number">$${(stats.totalRevenue || 0).toFixed(0)}</div>
                <div class="stat-label">Total Revenue</div>
              </div>
              <div class="stat-card" style="border-left: 3px solid ${(stats.pendingVerifications || 0) > 0 ? 'var(--warning, #f59e0b)' : 'var(--success, #22c55e)'};">
                <div class="stat-number">${stats.pendingVerifications || 0}</div>
                <div class="stat-label">Pending Verifications</div>
              </div>
            </div>

            <!-- Admin Tabs -->
            <div style="display:flex;gap:0.5rem;margin-bottom:1.5rem;flex-wrap:wrap;">
              <button class="btn btn-secondary btn-sm admin-tab-btn active" data-admintab="users" onclick="window.app.switchAdminTab('users')">👥 Users</button>
              <button class="btn btn-secondary btn-sm admin-tab-btn" data-admintab="verifications" onclick="window.app.switchAdminTab('verifications')">✅ Verifications ${(stats.pendingVerifications || 0) > 0 ? '<span style="background:var(--error);color:white;border-radius:50%;width:20px;height:20px;display:inline-flex;align-items:center;justify-content:center;font-size:0.7rem;margin-left:4px;">' + (stats.pendingVerifications || 0) + '</span>' : ''}</button>
              <button class="btn btn-secondary btn-sm admin-tab-btn" data-admintab="lessons" onclick="window.app.switchAdminTab('lessons')">📚 Lessons</button>
            </div>

            <!-- Users Tab -->
            <div id="admin-tab-users" class="admin-tab-content" style="display:block;">
              <div class="dashboard-card">
                <h3>All Users</h3>
                <div style="overflow-x: auto;">
                  <table style="width: 100%; border-collapse: collapse;">
                    <thead>
                      <tr style="border-bottom: 2px solid var(--gray-200);">
                        <th style="padding: 1rem; text-align: left;">Name</th>
                        <th style="padding: 1rem; text-align: left;">Email</th>
                        <th style="padding: 1rem; text-align: left;">Role</th>
                        <th style="padding: 1rem; text-align: left;">Status</th>
                        <th style="padding: 1rem; text-align: left;">Actions</th>
                      </tr>
                    </thead>
                    <tbody>
                      ${users.map(user => `
                        <tr style="border-bottom: 1px solid var(--gray-100);">
                          <td style="padding: 1rem;">${user.firstName} ${user.lastName}</td>
                          <td style="padding: 1rem;">${user.email}</td>
                          <td style="padding: 1rem;"><span class="status-badge" style="background:var(--gray-100);color:var(--gray-700);">${user.role}</span></td>
                          <td style="padding: 1rem;">
                            <span class="status-badge ${user.isActive ? 'status-confirmed' : 'status-cancelled'}">
                              ${user.isActive ? 'Active' : 'Inactive'}
                            </span>
                          </td>
                          <td style="padding: 1rem;">
                            <button class="btn btn-text btn-sm" onclick="window.app.toggleUserActive(${user.id})" style="font-size:0.8rem;">
                              ${user.isActive ? '🚫 Deactivate' : '✅ Activate'}
                            </button>
                          </td>
                        </tr>
                      `).join('')}
                    </tbody>
                  </table>
                </div>
              </div>
            </div>

            <!-- Verifications Tab -->
            <div id="admin-tab-verifications" class="admin-tab-content" style="display:none;">
              <div class="dashboard-card">
                <div style="display:flex;justify-content:space-between;align-items:center;margin-bottom:1rem;">
                  <h3>Teacher Verification Requests</h3>
                  <div style="display:flex;gap:0.5rem;">
                    <button class="btn btn-text btn-sm" onclick="window.app.loadVerifications('Pending')" style="font-weight:700;">Pending</button>
                    <button class="btn btn-text btn-sm" onclick="window.app.loadVerifications('Verified')">Verified</button>
                    <button class="btn btn-text btn-sm" onclick="window.app.loadVerifications('Rejected')">Rejected</button>
                  </div>
                </div>
                <div id="verificationsList">
                  ${pendingVerifications.length === 0 ? '<p style="text-align:center;color:var(--gray-500);padding:2rem;">🎉 No pending verifications</p>' : pendingVerifications.map(t => this.renderVerificationCard(t)).join('')}
                </div>
              </div>
            </div>

            <!-- Lessons Tab -->
            <div id="admin-tab-lessons" class="admin-tab-content" style="display:none;">
              <div class="dashboard-card">
                <h3>All Lessons</h3>
                <div style="overflow-x: auto;">
                  <table style="width: 100%; border-collapse: collapse;">
                    <thead>
                      <tr style="border-bottom: 2px solid var(--gray-200);">
                        <th style="padding: 1rem; text-align: left;">Teacher</th>
                        <th style="padding: 1rem; text-align: left;">Student</th>
                        <th style="padding: 1rem; text-align: left;">Subject</th>
                        <th style="padding: 1rem; text-align: left;">Date</th>
                        <th style="padding: 1rem; text-align: left;">Status</th>
                      </tr>
                    </thead>
                    <tbody>
                      ${lessons.slice(0, 20).map(l => `
                        <tr style="border-bottom: 1px solid var(--gray-100);">
                          <td style="padding: 1rem;">${l.teacherName}</td>
                          <td style="padding: 1rem;">${l.studentName}</td>
                          <td style="padding: 1rem;">${l.subject || 'N/A'}</td>
                          <td style="padding: 1rem;">${new Date(l.scheduledDateTime).toLocaleDateString()}</td>
                          <td style="padding: 1rem;"><span class="status-badge">${l.status}</span></td>
                        </tr>
                      `).join('')}
                    </tbody>
                  </table>
                </div>
              </div>
            </div>
          </div>
        </section>
      `;
    } catch (error) {
      console.error(error);
      this.container.innerHTML = '<div class="container" style="padding: 120px 0;"><p style="text-align: center; color: var(--error);">Failed to load admin dashboard</p></div>';
    }
  }

  switchAdminTab(tabName) {
    document.querySelectorAll('.admin-tab-content').forEach(el => el.style.display = 'none');
    document.querySelectorAll('.admin-tab-btn').forEach(btn => btn.classList.remove('active'));
    const tab = document.getElementById(`admin-tab-${tabName}`);
    if (tab) tab.style.display = 'block';
    const btn = document.querySelector(`.admin-tab-btn[data-admintab="${tabName}"]`);
    if (btn) btn.classList.add('active');
  }

  renderVerificationCard(t) {
    return `
      <div class="dashboard-card" style="margin-bottom:1rem;border-left:3px solid ${t.verificationStatus === 'Pending' ? '#f59e0b' : t.verificationStatus === 'Verified' ? '#22c55e' : '#ef4444'};">
        <div style="display:flex;justify-content:space-between;align-items:flex-start;gap:1rem;flex-wrap:wrap;">
          <div style="flex:1;min-width:200px;">
            <h4 style="margin:0 0 0.25rem;">${t.firstName} ${t.lastName}</h4>
            <p style="color:var(--gray-500);font-size:0.85rem;margin:0 0 0.5rem;">${t.email}</p>
            <p style="font-size:0.9rem;margin:0 0 0.5rem;">${t.bio}</p>
            <div style="display:flex;gap:1rem;flex-wrap:wrap;font-size:0.8rem;color:var(--gray-600);">
              <span>🎓 ${t.education || 'N/A'}</span>
              <span>📅 ${t.yearsOfExperience} years exp.</span>
              <span>📄 ${t.certificateUrl ? '<a href="' + t.certificateUrl + '" target="_blank" style="color:var(--primary);">View Certificate</a>' : 'No certificate'}</span>
            </div>
            ${t.verificationNotes ? '<p style="margin-top:0.5rem;font-size:0.8rem;color:var(--gray-500);">Notes: ' + t.verificationNotes + '</p>' : ''}
          </div>
          <div style="display:flex;gap:0.5rem;align-items:center;">
            <span class="status-badge" style="background:${t.verificationStatus === 'Pending' ? '#fef3c7' : t.verificationStatus === 'Verified' ? '#d1fae5' : '#fee2e2'};color:${t.verificationStatus === 'Pending' ? '#92400e' : t.verificationStatus === 'Verified' ? '#065f46' : '#991b1b'};">${t.verificationStatus}</span>
            ${t.verificationStatus === 'Pending' ? `
              <button class="btn btn-primary btn-sm" onclick="window.app.verifyTeacher(${t.teacherId}, 'Verified')">✅ Approve</button>
              <button class="btn btn-secondary btn-sm" style="color:var(--error);" onclick="window.app.verifyTeacher(${t.teacherId}, 'Rejected')">❌ Reject</button>
            ` : ''}
          </div>
        </div>
      </div>
    `;
  }

  async verifyTeacher(teacherId, status) {
    const notes = status === 'Rejected' ? prompt('Reason for rejection (optional):') : '';
    try {
      const result = await api.updateVerification(teacherId, status, notes || '');
      alert(result.message || 'Updated!');
      this.renderAdmin();
    } catch (err) {
      alert(err.message || 'Failed to update verification');
    }
  }

  async loadVerifications(status) {
    const list = document.getElementById('verificationsList');
    if (!list) return;
    list.innerHTML = '<div class="loading-container"><div class="spinner"></div></div>';
    try {
      const verifications = await api.getVerifications(status);
      list.innerHTML = verifications.length === 0
        ? `<p style="text-align:center;color:var(--gray-500);padding:2rem;">No ${status.toLowerCase()} verifications</p>`
        : verifications.map(t => this.renderVerificationCard(t)).join('');
    } catch (err) {
      list.innerHTML = '<p style="color:var(--error);text-align:center;">Failed to load</p>';
    }
  }

  async toggleUserActive(userId) {
    try {
      await api.toggleUserActive(userId);
      this.renderAdmin();
    } catch (err) {
      alert(err.message || 'Failed to toggle user status');
    }
  }



  async renderMessages(selectedUserId = null) {
    if (!AppState.isAuthenticated()) {
      authModule.openModal('login');
      return;
    }

    this.container.innerHTML = '<div class="loading-container"><div class="spinner"></div></div>';

    try {
      const conversations = await api.getConversations();
      let selectedConversation = null;
      let messages = [];

      // If a specific user is selected, get their conversation
      if (selectedUserId) {
        try {
          messages = await api.getConversation(selectedUserId);
          // Try to get user info - might be a teacher or any user
          try {
            const teacher = await api.getTeacher(selectedUserId);
            selectedConversation = {
              userId: selectedUserId,
              userName: `${teacher.firstName} ${teacher.lastName}`
            };
          } catch (e) {
            // If not a teacher, just use a generic name or get from conversation
            const existingConv = conversations.find(c => c.userId == selectedUserId);
            selectedConversation = {
              userId: selectedUserId,
              userName: existingConv?.userName || 'User'
            };
          }
        } catch (error) {
          console.error('Failed to load conversation:', error);
          // Still create a basic conversation object so the UI renders
          selectedConversation = {
            userId: selectedUserId,
            userName: 'New Conversation'
          };
          messages = [];
        }
      }

      this.container.innerHTML = `
        <section class="messages-page">
          <div class="container">
            <div class="messages-layout">
              <!-- Conversations List -->
              <div class="conversations-panel">
                <div class="conversations-header">
                  <h2>💬 Messages</h2>
                </div>
                <div class="conversations-list" id="conversationsList">
                  ${conversations.length > 0 ? conversations.map(conv => `
                    <div class="conversation-item ${selectedUserId == conv.userId ? 'active' : ''}" 
                         onclick="window.app.navigate('messages', ${conv.userId})">
                      <div class="conversation-avatar">${conv.userName.split(' ').map(n => n[0]).join('')}</div>
                      <div class="conversation-info">
                        <div class="conversation-name">${conv.userName}</div>
                        <div class="conversation-preview">${conv.lastMessage || 'No messages yet'}</div>
                      </div>
                      ${conv.unreadCount > 0 ? `<div class="unread-badge">${conv.unreadCount}</div>` : ''}
                    </div>
                  `).join('') : `
                    <div class="no-conversations">
                      <p>No conversations yet</p>
                      <small>${AppState.isTeacher() ? 'Start a conversation by messaging a student from your dashboard!' : 'Start a conversation by messaging a teacher!'}</small>
                    </div>
                  `}
                </div>
              </div>

              <!-- Chat Panel -->
              <div class="chat-panel">
                ${selectedConversation ? `
                  <div class="chat-header">
                    <div class="chat-user-info">
                      <div class="chat-avatar">${selectedConversation.userName.split(' ').map(n => n[0]).join('')}</div>
                      <div class="chat-user-name">${selectedConversation.userName}</div>
                    </div>
                  </div>
                  <div class="chat-messages" id="chatMessages">
                    ${messages.length > 0 ? messages.map(msg => `
                      <div class="message ${msg.senderId === AppState.currentUser.id ? 'sent' : 'received'}">
                        <div class="message-content">${msg.content}</div>
                        <div class="message-footer" style="display: flex; gap: 0.5rem; align-items: center; margin-top: 0.25rem; font-size: 0.7rem;">
                          <span class="message-sender" style="font-weight: 700; opacity: 0.9;">${msg.senderId === AppState.currentUser.id ? 'You' : selectedConversation.userName.split(' ')[0]}</span>
                          <span style="opacity: 0.5;">•</span>
                          <span class="message-time" style="opacity: 0.7;">${new Date(msg.sentAt).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}</span>
                        </div>
                      </div>
                    `).join('') : `
                      <div class="no-messages">
                        <p>No messages yet</p>
                        <small>Send a message to start the conversation!</small>
                      </div>
                    `}
                  </div>
                  <div class="chat-input-area">
                    <form id="messageForm" class="message-form">
                      <input type="text" id="messageInput" class="message-input" placeholder="Type your message..." autocomplete="off" required>
                      <button type="submit" class="btn btn-primary send-btn">
                        <span>Send</span>
                        <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                          <path d="M22 2L11 13M22 2l-7 20-4-9-9-4 20-7z"/>
                        </svg>
                      </button>
                    </form>
                  </div>
                ` : `
                  <div class="no-chat-selected">
                    <div class="no-chat-icon">💬</div>
                    <h3>Select a conversation</h3>
                    <p>${AppState.isTeacher() ? 'Choose a student from the list to start messaging' : 'Choose a teacher from the list or find a new one'}</p>
                    <button class="btn btn-primary" onclick="window.app.navigate('${AppState.isTeacher() ? 'dashboard' : 'teachers'}')">
                      ${AppState.isTeacher() ? '👥 Browse Students' : '🔍 Browse Teachers'}
                    </button>
                  </div>
                `}
              </div>
            </div>
          </div>
        </section>

        <style>
          .messages-page {
            padding-top: 90px;
            min-height: 100vh;
            background: var(--gray-50);
          }

          .messages-layout {
            display: grid;
            grid-template-columns: 320px 1fr;
            gap: 1.5rem;
            height: calc(100vh - 120px);
          }

          .conversations-panel {
            background: white;
            border-radius: var(--radius-xl);
            box-shadow: var(--shadow-md);
            display: flex;
            flex-direction: column;
            overflow: hidden;
          }

          .conversations-header {
            padding: 1.5rem;
            border-bottom: 1px solid var(--gray-200);
          }

          .conversations-header h2 {
            font-size: 1.25rem;
            font-weight: 700;
            color: var(--gray-900);
            margin: 0;
          }

          .conversations-list {
            flex: 1;
            overflow-y: auto;
          }

          .conversation-item {
            display: flex;
            align-items: center;
            gap: 1rem;
            padding: 1rem 1.5rem;
            cursor: pointer;
            transition: all 0.2s ease;
            border-bottom: 1px solid var(--gray-100);
          }

          .conversation-item:hover {
            background: var(--gray-50);
          }

          .conversation-item.active {
            background: linear-gradient(135deg, rgba(6, 182, 212, 0.1), rgba(59, 130, 246, 0.1));
            border-left: 3px solid var(--primary);
          }

          .conversation-avatar {
            width: 48px;
            height: 48px;
            border-radius: 50%;
            background: linear-gradient(135deg, var(--primary), var(--secondary));
            color: white;
            display: flex;
            align-items: center;
            justify-content: center;
            font-weight: 600;
            font-size: 1rem;
            flex-shrink: 0;
          }

          .conversation-info {
            flex: 1;
            min-width: 0;
          }

          .conversation-name {
            font-weight: 600;
            color: var(--gray-900);
            margin-bottom: 0.25rem;
          }

          .conversation-preview {
            font-size: 0.875rem;
            color: var(--gray-500);
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
          }

          .unread-badge {
            background: var(--primary);
            color: white;
            font-size: 0.75rem;
            font-weight: 600;
            padding: 0.25rem 0.5rem;
            border-radius: 9999px;
            min-width: 20px;
            text-align: center;
          }

          .no-conversations {
            padding: 3rem 1.5rem;
            text-align: center;
            color: var(--gray-500);
          }

          .chat-panel {
            background: white;
            border-radius: var(--radius-xl);
            box-shadow: var(--shadow-md);
            display: flex;
            flex-direction: column;
            overflow: hidden;
          }

          .chat-header {
            padding: 1rem 1.5rem;
            border-bottom: 1px solid var(--gray-200);
            background: var(--gray-50);
          }

          .chat-user-info {
            display: flex;
            align-items: center;
            gap: 1rem;
          }

          .chat-avatar {
            width: 40px;
            height: 40px;
            border-radius: 50%;
            background: linear-gradient(135deg, var(--primary), var(--secondary));
            color: white;
            display: flex;
            align-items: center;
            justify-content: center;
            font-weight: 600;
          }

          .chat-user-name {
            font-weight: 600;
            font-size: 1.1rem;
            color: var(--gray-900);
          }

          .chat-messages {
            flex: 1;
            overflow-y: auto;
            padding: 1.5rem;
            display: flex;
            flex-direction: column;
            gap: 1rem;
            width: 100%;
          }

          .message {
            max-width: 75%;
            padding: 0.85rem 1.25rem;
            border-radius: 1.25rem;
            position: relative;
            box-shadow: var(--shadow-sm);
            line-height: 1.5;
            transition: all 0.2s ease;
          }

          .message.sent {
            align-self: flex-end;
            background: linear-gradient(135deg, var(--primary), var(--secondary));
            color: white;
            border-bottom-right-radius: 0.25rem;
            box-shadow: 0 4px 12px rgba(6, 182, 212, 0.2);
          }

          .message.received {
            align-self: flex-start;
            background: white;
            color: var(--gray-800);
            border: 1px solid var(--gray-200);
            border-bottom-left-radius: 0.25rem;
          }

          .message-content {
            word-wrap: break-word;
            line-height: 1.4;
          }

          .message-footer {
            display: flex;
            align-items: center;
            gap: 0.4rem;
            margin-top: 0.4rem;
            padding-top: 0.3rem;
            border-top: 1px solid rgba(0, 0, 0, 0.05);
          }

          .message.sent .message-footer {
            border-top-color: rgba(255, 255, 255, 0.1);
            justify-content: flex-end;
          }

          .message-sender {
            font-weight: 700;
            font-size: 0.75rem;
          }

          .message-time {
            font-size: 0.7rem;
            opacity: 0.8;
          }

          .no-messages {
            flex: 1;
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            color: var(--gray-500);
            text-align: center;
          }

          .chat-input-area {
            padding: 1rem 1.5rem;
            border-top: 1px solid var(--gray-200);
            background: var(--gray-50);
          }

          .message-form {
            display: flex;
            gap: 0.75rem;
          }

          .message-input {
            flex: 1;
            padding: 0.875rem 1.25rem;
            border: 2px solid var(--gray-200);
            border-radius: 9999px;
            font-size: 1rem;
            transition: all 0.2s ease;
          }

          .message-input:focus {
            outline: none;
            border-color: var(--primary);
            box-shadow: 0 0 0 3px rgba(6, 182, 212, 0.1);
          }

          .send-btn {
            padding: 0.875rem 1.5rem;
            border-radius: 9999px;
            display: flex;
            align-items: center;
            gap: 0.5rem;
          }

          .send-btn svg {
            width: 18px;
            height: 18px;
          }

          .no-chat-selected {
            flex: 1;
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            text-align: center;
            padding: 2rem;
            color: var(--gray-600);
          }

          .no-chat-icon {
            font-size: 4rem;
            margin-bottom: 1rem;
          }

          .no-chat-selected h3 {
            font-size: 1.5rem;
            color: var(--gray-900);
            margin-bottom: 0.5rem;
          }

          .no-chat-selected p {
            margin-bottom: 1.5rem;
            max-width: 300px;
          }

          @media (max-width: 768px) {
            .messages-layout {
              grid-template-columns: 1fr;
            }

            .conversations-panel {
              ${selectedUserId ? 'display: none;' : ''}
            }

            .chat-panel {
              ${!selectedUserId ? 'display: none;' : ''}
            }
          }
        </style>
      `;

      // Setup message form submission
      if (selectedUserId) {
        const messageForm = document.getElementById('messageForm');
        const messageInput = document.getElementById('messageInput');
        const chatMessages = document.getElementById('chatMessages');

        messageForm?.addEventListener('submit', async (e) => {
          e.preventDefault();
          const content = messageInput.value.trim();
          if (!content) return;

          try {
            const newMessage = await api.sendMessage(parseInt(selectedUserId), content);

            // Add message to chat
            const messageEl = document.createElement('div');
            messageEl.className = 'message sent';
            messageEl.innerHTML = '<div class="message-content">' + content + '</div>' +
              '<div class="message-time">' + new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }) + '</div>';

            // Remove "no messages" placeholder if exists
            const noMessages = chatMessages.querySelector('.no-messages');
            if (noMessages) noMessages.remove();

            chatMessages.appendChild(messageEl);
            chatMessages.scrollTop = chatMessages.scrollHeight;
            messageInput.value = '';
          } catch (error) {
            alert('Failed to send message');
          }
        });

        // Scroll to bottom of messages
        if (chatMessages) {
          chatMessages.scrollTop = chatMessages.scrollHeight;
        }
      }
    } catch (error) {
      this.container.innerHTML = '<div class="container" style="padding: 120px 0;"><p style="text-align: center; color: var(--error);">Failed to load messages</p></div>';
    }
  }


  // ===== WHITEBOARD FEATURE =====
  async renderWhiteboard(roomId) {
    this.container.innerHTML = `
      <section class="whiteboard-page" style="padding-top: 100px;">
        <div class="container">
          <div class="section-header">
            <h2>Real-time Collaborative Whiteboard</h2>
            <p>Room: ${roomId}</p>
          </div>
          <div class="whiteboard-container">
            <div class="whiteboard-controls">
              <input type="color" id="brushColor" value="#06b6d4" title="Brush Color">
              <input type="range" id="brushSize" min="1" max="20" value="5" title="Brush Size">
              <button class="btn btn-secondary btn-sm" id="clearCanvas">🗑️ Clear</button>
              <button class="btn btn-secondary btn-sm" id="saveWhiteboardBtn">💾 Save to PC</button>
              <button class="btn btn-primary btn-sm" onclick="window.app.navigate('dashboard')">🏠 Back</button>
            </div>
            <canvas id="whiteboard" class="whiteboard-canvas" width="1000" height="600"></canvas>
          </div>
        </div>
      </section>
    `;

    this.initWhiteboard(roomId);
  }

  async initWhiteboard(roomId) {
    const canvas = document.getElementById('whiteboard');
    if (!canvas) return;
    const ctx = canvas.getContext('2d');
    const colorInput = document.getElementById('brushColor');
    const sizeInput = document.getElementById('brushSize');
    const clearBtn = document.getElementById('clearCanvas');

    let drawing = false;

    const hubConnection = new signalR.HubConnectionBuilder()
      .withUrl("/hubs/whiteboard")
      .build();

    await hubConnection.start();
    await hubConnection.invoke("JoinRoom", roomId);

    hubConnection.on("ReceiveDrawPath", (data) => {
      this.draw(ctx, data.x1, data.y1, data.x2, data.y2, data.color, data.size, false);
    });

    hubConnection.on("ReceiveClearCanvas", () => {
      ctx.clearRect(0, 0, canvas.width, canvas.height);
    });

    const getPos = (e) => {
      const rect = canvas.getBoundingClientRect();
      const clientX = e.touches ? e.touches[0].clientX : e.clientX;
      const clientY = e.touches ? e.touches[0].clientY : e.clientY;
      return {
        x: clientX - rect.left,
        y: clientY - rect.top
      };
    };

    let lastPos = { x: 0, y: 0 };

    const startDrawing = (e) => {
      drawing = true;
      lastPos = getPos(e);
    };

    const stopDrawing = () => {
      drawing = false;
    };

    const moveDrawing = (e) => {
      if (!drawing) return;
      const currentPos = getPos(e);
      const color = colorInput.value;
      const size = sizeInput.value;

      this.draw(ctx, lastPos.x, lastPos.y, currentPos.x, currentPos.y, color, size, true);

      hubConnection.invoke("DrawPath", roomId, {
        x1: lastPos.x, y1: lastPos.y,
        x2: currentPos.x, y2: currentPos.y,
        color, size
      });

      lastPos = currentPos;
    };

    canvas.addEventListener('mousedown', startDrawing);
    canvas.addEventListener('mousemove', moveDrawing);
    canvas.addEventListener('mouseup', stopDrawing);
    canvas.addEventListener('mouseout', stopDrawing);

    canvas.addEventListener('touchstart', (e) => { e.preventDefault(); startDrawing(e); });
    canvas.addEventListener('touchmove', (e) => { e.preventDefault(); moveDrawing(e); });
    canvas.addEventListener('touchend', stopDrawing);

    clearBtn.addEventListener('click', () => {
      if (confirm('Are you sure you want to clear the entire whiteboard?')) {
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        hubConnection.invoke("ClearCanvas", roomId);
      }
    });

    const saveBtn = document.getElementById('saveWhiteboardBtn');
    if (saveBtn) {
      saveBtn.addEventListener('click', () => {
        const link = document.createElement('a');
        link.download = `whiteboard-session-${roomId}-${new Date().getTime()}.png`;
        link.href = canvas.toDataURL('image/png');
        link.click();
        authModule.showSuccess('Whiteboard image saved to your downloads!');
      });
    }
  }

  draw(ctx, x1, y1, x2, y2, color, size, isLocal) {
    ctx.beginPath();
    ctx.strokeStyle = color;
    ctx.lineWidth = size;
    ctx.lineCap = 'round';
    ctx.moveTo(x1, y1);
    ctx.lineTo(x2, y2);
    ctx.stroke();
    ctx.closePath();
  }

  // ===== NOTEBOOK FEATURE =====
  async renderNotebook(lessonId) {
    this.container.innerHTML = '<div class="loading-container"><div class="spinner"></div></div>';

    try {
      const notebook = await api.getOrCreateNotebookByLesson(lessonId);

      this.container.innerHTML = `
        <section class="notebook-page" style="padding-top: 100px;">
          <div class="container">
            <div class="section-header" style="display:flex; justify-content:space-between; align-items:center;">
              <div>
                <h2>Shared Lesson Notebook</h2>
                <p>Collaborate in real-time with your ${AppState.isTeacher() ? 'student' : 'teacher'}</p>
              </div>
              <button class="btn btn-primary btn-sm" onclick="window.app.navigate('dashboard')">Save & Close</button>
            </div>
            <div class="notebook-container">
              <div class="notebook-editor">
                <textarea id="notebookArea" class="notebook-textarea" placeholder="Type your notes here... support markdown">${notebook.content}</textarea>
              </div>
            </div>
          </div>
        </section>
      `;

      this.initNotebook(notebook.id);
    } catch (e) {
      this.container.innerHTML = '<p>Error loading notebook</p>';
    }
  }

  async initNotebook(notebookId) {
    const textarea = document.getElementById('notebookArea');
    if (!textarea) return;

    const hubConnection = new signalR.HubConnectionBuilder()
      .withUrl("/hubs/notebook")
      .build();

    await hubConnection.start();
    await hubConnection.invoke("JoinNotebook", notebookId);

    hubConnection.on("ReceiveContentUpdate", (content) => {
      // Simple sync - in a production app, use OT or CRDTs for smoother multi-user editing
      const cursor = textarea.selectionStart;
      textarea.value = content;
      textarea.setSelectionRange(cursor, cursor);
    });

    let timeout = null;
    textarea.addEventListener('input', () => {
      clearTimeout(timeout);
      timeout = setTimeout(() => {
        hubConnection.invoke("UpdateContent", notebookId, textarea.value);
      }, 500);
    });
  }

  // ===== MEETINGS FEATURE =====
  async renderMeetings() {
    this.container.innerHTML = '<div class="loading-container"><div class="spinner"></div></div>';
    try {
      const meetings = await api.getMyMeetings();
      this.container.innerHTML = `
        <section class="meetings-page" style="padding-top: 100px;">
          <div class="container">
            <div class="section-header">
              <h2>Parent-Teacher Meeting Requests</h2>
              <p>Manage your 10-minute check-in requests</p>
            </div>
            <div class="meeting-list">
              ${meetings.length ? meetings.map(m => `
                <div class="meeting-card">
                  <div class="meeting-info">
                    <h4>Meeting regarding student: ${m.studentName}</h4>
                    <p><strong>With:</strong> ${AppState.isTeacher() ? m.parentName : m.teacherName}</p>
                    <p><strong>Time:</strong> ${new Date(m.requestedDateTime).toLocaleString()}</p>
                    <p><strong>Reason:</strong> ${m.reason || 'No reason provided'}</p>
                  </div>
                  <div class="meeting-actions">
                    <span class="meeting-status status-${m.status.toLowerCase()}">${m.status}</span>
                    ${AppState.isTeacher() && m.status === 'Pending' ? `
                      <div style="margin-top: 1rem;">
                        <button class="btn btn-primary btn-sm" onclick="window.app.updateMeetingStatus(${m.id}, 'Accepted')">Accept</button>
                        <button class="btn btn-secondary btn-sm" onclick="window.app.updateMeetingStatus(${m.id}, 'Declined')">Decline</button>
                      </div>
                    ` : ''}
                  </div>
                </div>
              `).join('') : '<p>No meeting requests found.</p>'}
            </div>
          </div>
        </section>
      `;
    } catch (e) {
      this.container.innerHTML = '<p>Error loading meetings</p>';
    }
  }

  async updateMeetingStatus(id, status) {
    try {
      await api.updateMeetingStatus(id, status);
      this.renderMeetings();
    } catch (e) {
      alert('Failed to update status');
    }
  }

  renderPrivacyPolicy() {
    this.container.innerHTML = `
      <section class="legal-page" style="padding-top: 120px; padding-bottom: 80px;">
        <div class="container">
          <div class="legal-content-card">
            <h1 class="section-title">Privacy Policy</h1>
            <p class="last-updated">Last Updated: March 2024</p>
            
            <div class="policy-body">
              <h3>1. Information We Collect</h3>
              <p>At LearnConnect, we collect information that you provide directly to us when you create an account, book a lesson, or communicate with other users. This includes your name, email address, profile picture, and educational background.</p>

              <h3>2. How We Use Your Information</h3>
              <p>We use the information we collect to provider, maintain, and improve our services, including to facilitate bookings between students and teachers, process payments, and send you technical notices and support messages.</p>

              <h3>3. Data Security</h3>
              <p>We implement a variety of security measures to maintain the safety of your personal information. Your personal information is contained behind secured networks and is only accessible by a limited number of persons who have special access rights to such systems.</p>

              <h3>4. Cookies</h3>
              <p>We use cookies to help us remember and process the items in your shopping cart and understand and save your preferences for future visits.</p>

              <h3>5. Third-Party Disclosure</h3>
              <p>We do not sell, trade, or otherwise transfer to outside parties your Personally Identifiable Information unless we provide users with advance notice. This does not include website hosting partners and other parties who assist us in operating our website, so long as those parties agree to keep this information confidential.</p>

              <h3>6. Contact Us</h3>
              <p>If there are any questions regarding this privacy policy, you may contact us using the information in our footer.</p>
            </div>
          </div>
        </div>
      </section>
    `;
    window.scrollTo(0, 0);
  }

  renderTermsOfService() {
    this.container.innerHTML = `
      <section class="legal-page" style="padding-top: 120px; padding-bottom: 80px;">
        <div class="container">
          <div class="legal-content-card">
            <h1 class="section-title">Terms of Service</h1>
            <p class="last-updated">Last Updated: March 2024</p>
            
            <div class="policy-body">
              <h3>1. Acceptance of Terms</h3>
              <p>By accessing or using LearnConnect, you agree to be bound by these Terms of Service and all applicable laws and regulations. If you do not agree with any of these terms, you are prohibited from using or accessing this site.</p>

              <h3>2. User Account</h3>
              <p>To access certain features of the Service, you must register for an account. You are responsible for maintaining the confidentiality of your account credentials and for all activities that occur under your account.</p>

              <h3>3. Booking and Payments</h3>
              <p>LearnConnect facilitates the booking of lessons between students and teachers. All payments are processed through our secure third-party payment provider. Teachers are independent contractors and are not employees of LearnConnect.</p>

              <h3>4. Cancellation Policy</h3>
              <p>Users must adhere to the cancellation policy specified at the time of booking. LearnConnect reserves the right to charge fees for cancellations made within the restricted timeframe.</p>

              <h3>5. Prohibited Conduct</h3>
              <p>Users agree not to use the service for any unlawful purpose or in any way that interrupts, damages, or impairs the service. Harassment of other users is strictly prohibited.</p>

              <h3>6. Limitation of Liability</h3>
              <p>In no event shall LearnConnect be liable for any damages arising out of the use or inability to use the materials on LearnConnect's website.</p>

              <h3>7. Governing Law</h3>
              <p>These terms and conditions are governed by and construed in accordance with the laws of the jurisdiction in which LearnConnect operates.</p>
            </div>
          </div>
        </div>
      </section>
    `;
    window.scrollTo(0, 0);
  }
}

// Initialize app when DOM is ready
window.addEventListener('DOMContentLoaded', () => {
  window.app = new App();
});
