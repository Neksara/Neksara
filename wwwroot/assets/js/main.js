/**
* Template Name: Arsha
* Template URL: https://bootstrapmade.com/arsha-free-bootstrap-html-template-corporate/
* Updated: Feb 22 2025 with Bootstrap v5.3.3
* Author: BootstrapMade.com
* License: https://bootstrapmade.com/license/
*/

(function() {
  "use strict";

  /**
   * Apply .scrolled class to the body as the page is scrolled down
   */
  function toggleScrolled() {
    const selectBody = document.querySelector('body');
    const selectHeader = document.querySelector('#header');
    if (!selectHeader.classList.contains('scroll-up-sticky') && !selectHeader.classList.contains('sticky-top') && !selectHeader.classList.contains('fixed-top')) return;
    window.scrollY > 100 ? selectBody.classList.add('scrolled') : selectBody.classList.remove('scrolled');
  }

  document.addEventListener('scroll', toggleScrolled);
  window.addEventListener('load', toggleScrolled);

  /**
   * Mobile nav toggle
   */
  const mobileNavToggleBtn = document.querySelector('.mobile-nav-toggle');

  function mobileNavToogle() {
    document.querySelector('body').classList.toggle('mobile-nav-active');
    mobileNavToggleBtn.classList.toggle('bi-list');
    mobileNavToggleBtn.classList.toggle('bi-x');
  }
  if (mobileNavToggleBtn) {
    mobileNavToggleBtn.addEventListener('click', mobileNavToogle);
  }

  /**
   * Hide mobile nav on same-page/hash links
   */
  document.querySelectorAll('#navmenu a').forEach(navmenu => {
    navmenu.addEventListener('click', () => {
      if (document.querySelector('.mobile-nav-active')) {
        mobileNavToogle();
      }
    });

  });

  /**
   * Toggle mobile nav dropdowns
   */
  document.querySelectorAll('.navmenu .toggle-dropdown').forEach(navmenu => {
    navmenu.addEventListener('click', function(e) {
      e.preventDefault();
      this.parentNode.classList.toggle('active');
      this.parentNode.nextElementSibling.classList.toggle('dropdown-active');
      e.stopImmediatePropagation();
    });
  });

  /**
   * Preloader
   */
  const preloader = document.querySelector('#preloader');
  if (preloader) {
    window.addEventListener('load', () => {
      preloader.remove();
    });
  }

  /**
   * Scroll top button
   */
  let scrollTop = document.querySelector('.scroll-top');

  function toggleScrollTop() {
    if (scrollTop) {
      window.scrollY > 100 ? scrollTop.classList.add('active') : scrollTop.classList.remove('active');
    }
  }
  scrollTop.addEventListener('click', (e) => {
    e.preventDefault();
    window.scrollTo({
      top: 0,
      behavior: 'smooth'
    });
  });

  window.addEventListener('load', toggleScrollTop);
  document.addEventListener('scroll', toggleScrollTop);

  /**
   * Animation on scroll function and init
   */
  function aosInit() {
    AOS.init({
      duration: 600,
      easing: 'ease-in-out',
      once: true,
      mirror: false
    });
  }
  window.addEventListener('load', aosInit);

  /**
   * Initiate glightbox
   */
  const glightbox = GLightbox({
    selector: '.glightbox'
  });

  /**
   * Init swiper sliders
   */
  function initSwiper() {
    document.querySelectorAll(".init-swiper").forEach(function(swiperElement) {
      let config = JSON.parse(
        swiperElement.querySelector(".swiper-config").innerHTML.trim()
      );

      if (swiperElement.classList.contains("swiper-tab")) {
        initSwiperWithCustomPagination(swiperElement, config);
      } else {
        new Swiper(swiperElement, config);
      }
    });
  }

  window.addEventListener("load", initSwiper);

  /**
   * Frequently Asked Questions Toggle
   */
  document.querySelectorAll('.faq-item h3, .faq-item .faq-toggle').forEach((faqItem) => {
    faqItem.addEventListener('click', () => {
      faqItem.parentNode.classList.toggle('faq-active');
    });
  });

  /**
   * Animate the skills items on reveal
   */
  let skillsAnimation = document.querySelectorAll('.skills-animation');
  skillsAnimation.forEach((item) => {
    new Waypoint({
      element: item,
      offset: '80%',
      handler: function(direction) {
        let progress = item.querySelectorAll('.progress .progress-bar');
        progress.forEach(el => {
          el.style.width = el.getAttribute('aria-valuenow') + '%';
        });
      }
    });
  });

  /**
   * Init isotope layout and filters
   */
  document.querySelectorAll('.isotope-layout').forEach(function(isotopeItem) {
    let layout = isotopeItem.getAttribute('data-layout') ?? 'masonry';
    let filter = isotopeItem.getAttribute('data-default-filter') ?? '*';
    let sort = isotopeItem.getAttribute('data-sort') ?? 'original-order';

    let initIsotope;
    imagesLoaded(isotopeItem.querySelector('.isotope-container'), function() {
      initIsotope = new Isotope(isotopeItem.querySelector('.isotope-container'), {
        itemSelector: '.isotope-item',
        layoutMode: layout,
        filter: filter,
        sortBy: sort
      });
    });

    isotopeItem.querySelectorAll('.isotope-filters li').forEach(function(filters) {
      filters.addEventListener('click', function() {
        isotopeItem.querySelector('.isotope-filters .filter-active').classList.remove('filter-active');
        this.classList.add('filter-active');
        initIsotope.arrange({
          filter: this.getAttribute('data-filter')
        });
        if (typeof aosInit === 'function') {
          aosInit();
        }
      }, false);
    });

  });

  /**
   * Correct scrolling position upon page load for URLs containing hash links.
   */
  window.addEventListener('load', function(e) {
    if (window.location.hash) {
      if (document.querySelector(window.location.hash)) {
        setTimeout(() => {
          let section = document.querySelector(window.location.hash);
          let scrollMarginTop = getComputedStyle(section).scrollMarginTop;
          window.scrollTo({
            top: section.offsetTop - parseInt(scrollMarginTop),
            behavior: 'smooth'
          });
        }, 100);
      }
    }
  });

  /**
   * Navmenu Scrollspy
   */
  let navmenulinks = document.querySelectorAll('.navmenu a');

  function navmenuScrollspy() {
    navmenulinks.forEach(navmenulink => {
      if (!navmenulink.hash) return;
      let section = document.querySelector(navmenulink.hash);
      if (!section) return;
      let position = window.scrollY + 200;
      if (position >= section.offsetTop && position <= (section.offsetTop + section.offsetHeight)) {
        document.querySelectorAll('.navmenu a.active').forEach(link => link.classList.remove('active'));
        navmenulink.classList.add('active');
      } else {
        navmenulink.classList.remove('active');
      }
    })
  }
  window.addEventListener('load', navmenuScrollspy);
  document.addEventListener('scroll', navmenuScrollspy);

  /**
   * Contact modal trigger
   */
  function initContactModal() {
    const modalEl = document.getElementById('contactModal');
    if (!modalEl) return;
    const bsModal = new bootstrap.Modal(modalEl, { keyboard: true });
    if (window.console) console.log('initContactModal: modal found, binding triggers');

    document.querySelectorAll('.open-contact-modal').forEach(el => {
        // Add data-API attributes as a fallback (Bootstrap will handle them)
        el.setAttribute('data-bs-toggle', 'modal');
        el.setAttribute('data-bs-target', '#contactModal');
        el.addEventListener('click', function(e) {
          e.preventDefault();
          if (window.console) console.log('initContactModal: trigger clicked');
          bsModal.show();
        });
    });

    const sendBtn = document.getElementById('contactSendBtn');
    const form = document.getElementById('contactModalForm');
    if (sendBtn && form) {
      sendBtn.addEventListener('click', function() {
        if (!form.checkValidity()) {
          form.reportValidity();
          return;
        }
        sendBtn.disabled = true;
        sendBtn.innerText = 'Sending...';
        // Simulate send; replace with fetch POST to server if needed
        setTimeout(() => {
          sendBtn.disabled = false;
          sendBtn.innerText = 'Send';
          form.reset();
          bsModal.hide();
          // simple feedback
          alert('Pesan terkirim. Terima kasih!');
            if (window.console) console.log('Contact form simulated send.');
        }, 800);
      });
    }
  }
  window.addEventListener('load', initContactModal);

  /**
   * A-Z display-only filter for categories
   */
  function initAZFilter() {
    const row = document.getElementById('categories-row');
    if (!row) return;
    const buttons = document.querySelectorAll('#az-filter .az-letter');

    function setActive(btn) {
      buttons.forEach(b => b.classList.remove('active'));
      btn.classList.add('active');
    }

    function filterByLetter(letter) {
      const cols = Array.from(row.children).filter(n => n.nodeType === 1);
      cols.forEach(col => {
        const titleEl = col.querySelector('.service-item h4') || col.querySelector('h4');
        const title = titleEl ? titleEl.textContent.trim() : '';
        if (!title) {
          col.style.display = '';
          return;
        }
        if (letter === 'all') {
          col.style.display = '';
        } else {
          col.style.display = title.charAt(0).toUpperCase() === letter ? '' : 'none';
        }
      });
    }

    buttons.forEach(btn => {
      btn.addEventListener('click', function(e) {
        e.preventDefault();
        const letter = this.getAttribute('data-letter');
        setActive(this);
        filterByLetter(letter);
        // update dropdown label when used inside dropdown
        const toggle = document.getElementById('azDropdown');
        if (toggle) toggle.innerText = 'Filter: ' + (letter === 'all' ? 'All' : letter);
      });
    });
  }
  window.addEventListener('load', initAZFilter);

})();

(function () {
  const track = document.querySelector('.topics-track');
  if (!track) return;

  let index = 0;
  const cardWidth = track.querySelector('.topic-card-item')?.offsetWidth || 300;

  document.querySelector('.arrow-btn.next')?.addEventListener('click', () => {
    index++;
    track.style.transform = `translateX(-${index * cardWidth}px)`;
  });

  document.querySelector('.arrow-btn.prev')?.addEventListener('click', () => {
    index = Math.max(index - 1, 0);
    track.style.transform = `translateX(-${index * cardWidth}px)`;
  });
})();

(function () {
  const track = document.querySelector('.topics-track');
  const prevBtn = document.querySelector('.arrow-btn.prev');
  const nextBtn = document.querySelector('.arrow-btn.next');

  if (!track || !prevBtn || !nextBtn) return;

  const cards = track.querySelectorAll('.topic-card-item');
  const cardWidth = cards[0].offsetWidth;
  const visibleCards = 4;
  const maxIndex = cards.length - visibleCards;

  let index = 0;

  function updateSlider() {
    track.style.transform = `translateX(-${index * cardWidth}px)`;
    prevBtn.classList.toggle('active', index > 0);
    nextBtn.classList.toggle('active', index < maxIndex);
  }

  nextBtn.addEventListener('click', () => {
    if (index < maxIndex) {
      index++;
      updateSlider();
    }
  });

  prevBtn.addEventListener('click', () => {
    if (index > 0) {
      index--;
      updateSlider();
    }
  });

  updateSlider();
})();

// Simple Testimonials Slider
(function() {
  const track = document.getElementById('testimonial-track');
  const prevBtn = document.getElementById('testimonial-prev');
  const nextBtn = document.getElementById('testimonial-next');
  
  if (!track || !prevBtn || !nextBtn) return;
  
  let currentIndex = 0;
  let cardWidth = 0;
  let maxIndex = 0;
  
  function calculateSlider() {
    const cards = track.querySelectorAll('.testimonial-card-simple');
    if (cards.length === 0) return;
    
    // Calculate card width (including gap)
    cardWidth = cards[0].offsetWidth + 25; // width + gap
    
    // Calculate container width
    const containerWidth = track.parentElement.offsetWidth;
    
    // Calculate how many cards can fit
    const visibleCards = Math.floor(containerWidth / cardWidth);
    
    // Calculate max index
    maxIndex = Math.max(0, cards.length - visibleCards);
    
    // Update slider position
    updateSlider();
  }
  
  function updateSlider() {
    track.style.transform = `translateX(-${currentIndex * cardWidth}px)`;
    
    // Update button states
    prevBtn.classList.toggle('disabled', currentIndex === 0);
    nextBtn.classList.toggle('disabled', currentIndex >= maxIndex);
  }
  
  // Event Listeners
  nextBtn.addEventListener('click', () => {
    if (currentIndex < maxIndex) {
      currentIndex++;
      updateSlider();
    }
  });
  
  prevBtn.addEventListener('click', () => {
    if (currentIndex > 0) {
      currentIndex--;
      updateSlider();
    }
  });
  
  // Initialize on load
  window.addEventListener('load', () => {
    setTimeout(calculateSlider, 100); // Small delay to ensure rendering
  });
  
  // Recalculate on resize
  let resizeTimer;
  window.addEventListener('resize', () => {
    clearTimeout(resizeTimer);
    resizeTimer = setTimeout(calculateSlider, 250);
  });
  
  // Touch/swipe support
  let startX = 0;
  let isDragging = false;
  
  track.addEventListener('touchstart', (e) => {
    startX = e.touches[0].clientX;
    isDragging = true;
  });
  
  track.addEventListener('touchmove', (e) => {
    if (!isDragging) return;
    e.preventDefault();
  });
  
  track.addEventListener('touchend', (e) => {
    if (!isDragging) return;
    isDragging = false;
    
    const endX = e.changedTouches[0].clientX;
    const diffX = startX - endX;
    
    // Minimum swipe distance
    if (Math.abs(diffX) > 50) {
      if (diffX > 0 && currentIndex < maxIndex) {
        // Swipe left - next
        currentIndex++;
      } else if (diffX < 0 && currentIndex > 0) {
        // Swipe right - previous
        currentIndex--;
      }
      updateSlider();
    }
  });
  
})();

// Tambahkan ke file site.js
document.addEventListener('DOMContentLoaded', function() {
    // Inisialisasi filter A-Z untuk kategori
    initCategoryFilter();
});

function initCategoryFilter() {
    const azLetters = document.querySelectorAll('.az-letter');
    const categoryItems = document.querySelectorAll('.category-item');
    const filterButton = document.querySelector('#azDropdown');
    
    // Hitung jumlah kategori per huruf
    const categoryCounts = {};
    categoryItems.forEach(item => {
        const letter = item.getAttribute('data-category-letter');
        categoryCounts[letter] = (categoryCounts[letter] || 0) + 1;
    });
    
    // Tambahkan jumlah ke dropdown (opsional)
    azLetters.forEach(letter => {
        const letterChar = letter.getAttribute('data-letter');
        if (letterChar !== 'all' && categoryCounts[letterChar]) {
            const badge = document.createElement('span');
            badge.className = 'category-count-badge';
            badge.textContent = categoryCounts[letterChar];
            letter.appendChild(badge);
        }
    });
    
    // Event listener untuk filter
    azLetters.forEach(letter => {
        letter.addEventListener('click', function(e) {
            e.preventDefault();
            
            // Hapus active class dari semua
            azLetters.forEach(l => l.classList.remove('active'));
            
            // Tambah active class ke yang diklik
            this.classList.add('active');
            
            // Update teks tombol
            const selectedLetter = this.getAttribute('data-letter');
            const displayText = selectedLetter === 'all' ? 'All' : selectedLetter;
            filterButton.innerHTML = `<i class="bi bi-filter-circle me-2"></i>Filter: ${displayText}`;
            
            // Filter kategori
            filterCategories(selectedLetter);
            
            // Tutup dropdown setelah memilih
            const dropdown = bootstrap.Dropdown.getInstance(filterButton);
            dropdown?.hide();
        });
    });
    
    function filterCategories(letter) {
        categoryItems.forEach(item => {
            const itemLetter = item.getAttribute('data-category-letter');
            
            if (letter === 'all' || itemLetter === letter) {
                // Tampilkan item
                item.classList.remove('hidden');
                setTimeout(() => {
                    item.style.display = 'block';
                    // Trigger animation
                    item.classList.add('visible');
                }, 10);
            } else {
                // Sembunyikan item
                item.classList.remove('visible');
                item.classList.add('hidden');
                setTimeout(() => {
                    item.style.display = 'none';
                }, 400);
            }
        });
        
        // Update AOS untuk item yang ditampilkan
        setTimeout(() => {
            if (typeof AOS !== 'undefined') {
                AOS.refresh();
            }
        }, 500);
    }
    
    // Reset filter jika diklik di luar
    document.addEventListener('click', function(e) {
        if (!e.target.closest('#az-filter')) {
            // Bisa tambahkan reset logic jika diperlukan
        }
    });
    
    // Fungsi untuk mencari kategori
    function searchCategories(query) {
        categoryItems.forEach(item => {
            const title = item.querySelector('h4').textContent.toLowerCase();
            const description = item.querySelector('p').textContent.toLowerCase();
            
            if (title.includes(query.toLowerCase()) || description.includes(query.toLowerCase())) {
                item.style.display = 'block';
                item.classList.add('visible');
                item.classList.remove('hidden');
            } else {
                item.classList.remove('visible');
                item.classList.add('hidden');
                setTimeout(() => {
                    item.style.display = 'none';
                }, 400);
            }
        });
    }
    
    // Event listener untuk pencarian (opsional)
    const searchInput = document.querySelector('#category-search');
    if (searchInput) {
        searchInput.addEventListener('input', function(e) {
            searchCategories(e.target.value);
        });
    }
}

// Filter untuk Topics Section
function initTopicsFilter() {
    const topicItems = document.querySelectorAll('.topic-item');
    const filterPopularBtn = document.querySelector('.filter-popular');
    const categoryFilterItems = document.querySelectorAll('.category-filter');
    const azFilterItems = document.querySelectorAll('.az-filter');
    
    if (topicItems.length === 0) return;
    
    let activeFilters = {
        popular: 'all',
        category: 'all',
        letter: 'all'
    };
    
    // Filter Populer
    if (filterPopularBtn) {
        filterPopularBtn.addEventListener('click', function() {
            const isActive = this.classList.contains('active');
            const filterValue = isActive ? 'false' : 'true';
            
            // Toggle active state
            if (isActive) {
                this.classList.remove('active');
                activeFilters.popular = 'all';
            } else {
                this.classList.add('active');
                activeFilters.popular = 'true';
            }
            
            applyFilters();
        });
    }
    
    // Filter Kategori
    categoryFilterItems.forEach(item => {
        item.addEventListener('click', function(e) {
            e.preventDefault();
            
            // Hapus active class dari semua
            categoryFilterItems.forEach(i => i.classList.remove('active'));
            
            // Tambah active class ke yang diklik
            this.classList.add('active');
            
            // Update filter
            activeFilters.category = this.getAttribute('data-category');
            applyFilters();
            
            // Tutup dropdown
            const dropdown = bootstrap.Dropdown.getInstance(this.closest('.dropdown').querySelector('button'));
            dropdown?.hide();
        });
    });
    
    // Filter A-Z
    azFilterItems.forEach(item => {
        item.addEventListener('click', function(e) {
            e.preventDefault();
            
            // Hapus active class dari semua
            azFilterItems.forEach(i => i.classList.remove('active'));
            
            // Tambah active class ke yang diklik
            this.classList.add('active');
            
            // Update filter
            activeFilters.letter = this.getAttribute('data-letter') || 'all';
            applyFilters();
            
            // Tutup dropdown
            const dropdown = bootstrap.Dropdown.getInstance(this.closest('.dropdown').querySelector('button'));
            dropdown?.hide();
        });
    });
    
    // Fungsi untuk menerapkan semua filter
    function applyFilters() {
        let visibleCount = 0;
        
        topicItems.forEach(item => {
            const isPopular = item.getAttribute('data-popular') === 'true';
            const itemCategory = item.getAttribute('data-category');
            const itemLetter = item.getAttribute('data-topic-letter');
            
            // Cek semua kondisi filter
            const matchPopular = activeFilters.popular === 'all' || 
                               (activeFilters.popular === 'true' && isPopular);
            
            const matchCategory = activeFilters.category === 'all' || 
                                 activeFilters.category === itemCategory;
            
            const matchLetter = activeFilters.letter === 'all' || 
                               activeFilters.letter === itemLetter;
            
            if (matchPopular && matchCategory && matchLetter) {
                // Tampilkan item
                item.classList.remove('filtered-out');
                item.classList.add('filtered-in');
                item.style.display = 'block';
                visibleCount++;
            } else {
                // Sembunyikan item
                item.classList.remove('filtered-in');
                item.classList.add('filtered-out');
                setTimeout(() => {
                    item.style.display = 'none';
                }, 400);
            }
        });
        
        // Tampilkan pesan jika tidak ada hasil
        showNoResultsMessage(visibleCount);
        
        // Refresh AOS
        setTimeout(() => {
            if (typeof AOS !== 'undefined') {
                AOS.refresh();
            }
        }, 500);
    }
    
    function showNoResultsMessage(count) {
        const topicsRow = document.getElementById('topics-row');
        let message = topicsRow.querySelector('.no-results-message');
        
        if (count === 0) {
            if (!message) {
                message = document.createElement('div');
                message.className = 'no-results-message col-12 text-center py-5';
                message.innerHTML = `
                    <i class="bi bi-search display-4 text-muted mb-3"></i>
                    <h4 class="text-muted mb-2">Tidak ada topik yang ditemukan</h4>
                    <p class="text-muted">Coba gunakan filter yang berbeda atau reset filter.</p>
                    <button class="btn btn-primary mt-3" id="resetFilters">
                        Reset Semua Filter
                    </button>
                `;
                topicsRow.appendChild(message);
                
                // Reset filter button
                document.getElementById('resetFilters').addEventListener('click', resetAllFilters);
            }
        } else if (message) {
            message.remove();
        }
    }
    
    function resetAllFilters() {
        // Reset popular filter
        if (filterPopularBtn) {
            filterPopularBtn.classList.add('active');
            activeFilters.popular = 'all';
        }
        
        // Reset category filter
        categoryFilterItems.forEach(item => {
            item.classList.remove('active');
            if (item.getAttribute('data-category') === 'all') {
                item.classList.add('active');
            }
        });
        activeFilters.category = 'all';
        
        // Reset A-Z filter
        azFilterItems.forEach(item => {
            item.classList.remove('active');
            if (item.getAttribute('data-sort') === 'all' || !item.getAttribute('data-letter')) {
                item.classList.add('active');
            }
        });
        activeFilters.letter = 'all';
        
        applyFilters();
    }
    
    // Inisialisasi awal
    applyFilters();
}

// Panggil fungsi saat DOM siap
document.addEventListener('DOMContentLoaded', function() {
    initTopicsFilter();
});