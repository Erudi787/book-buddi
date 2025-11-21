// Star Rating Component JavaScript

// Initialize star rating display
function displayStarRating(rating, maxStars = 5) {
    const fullStars = Math.floor(rating);
    const hasHalfStar = rating % 1 >= 0.5;
    const emptyStars = maxStars - fullStars - (hasHalfStar ? 1 : 0);
    
    let starsHTML = '';
    
    // Full stars
    for (let i = 0; i < fullStars; i++) {
        starsHTML += '<span class="star filled">★</span>';
    }
    
    // Half star
    if (hasHalfStar) {
        starsHTML += '<span class="star half-filled">★</span>';
    }
    
    // Empty stars
    for (let i = 0; i < emptyStars; i++) {
        starsHTML += '<span class="star">☆</span>';
    }
    
    return starsHTML;
}

// Initialize interactive star rating
function initializeStarRating(containerId, currentRating = 0, onChange = null) {
    const container = document.getElementById(containerId);
    if (!container) return;
    
    const stars = container.querySelectorAll('.star.interactive');
    let selectedRating = currentRating;
    
    stars.forEach((star, index) => {
        const rating = index + 1;
        
        // Click to select rating
        star.addEventListener('click', () => {
            selectedRating = rating;
            updateStars(selectedRating);
            if (onChange) onChange(selectedRating);
        });
        
        // Hover preview
        star.addEventListener('mouseenter', () => {
            updateStars(rating);
        });
        
        // Reset to selected on mouse leave
        container.addEventListener('mouseleave', () => {
            updateStars(selectedRating);
        });
    });
    
    function updateStars(rating) {
        stars.forEach((star, index) => {
            if (index < rating) {
                star.classList.add('filled');
            } else {
                star.classList.remove('filled');
            }
        });
    }
    
    // Initialize display
    updateStars(currentRating);
    
    return {
        getRating: () => selectedRating,
        setRating: (rating) => {
            selectedRating = rating;
            updateStars(rating);
        }
    };
}

// Rating Modal
let ratingModal = null;
let currentBookId = null;
let currentRatingId = null;
let currentScore = 0;

function showRatingModal(bookId, bookTitle, existingRating = 0, ratingId = null) {
    currentBookId = bookId;
    currentRatingId = ratingId;
    currentScore = existingRating;
    
    const modal = document.getElementById('ratingModal');
    if (!modal) return;
    
    const titleElement = modal.querySelector('.rating-modal-header h3');
    if (titleElement) {
        titleElement.textContent = existingRating > 0 ? 'Edit Your Rating' : 'Rate This Book';
    }
    
    const subtitleElement = modal.querySelector('.rating-modal-header p');
    if (subtitleElement) {
        subtitleElement.textContent = bookTitle;
    }
    
    // Initialize stars
    ratingModal = initializeStarRating('ratingModalStars', existingRating, (rating) => {
        currentScore = rating;
    });
    
    modal.classList.add('show');
}

function closeRatingModal() {
    const modal = document.getElementById('ratingModal');
    if (modal) {
        modal.classList.remove('show');
    }
    currentBookId = null;
    currentRatingId = null;
    currentScore = 0;
}

async function submitRating() {
    if (!currentBookId || currentScore === 0) {
        alert('Please select a rating');
        return;
    }
    
    const submitBtn = document.querySelector('.btn-rate-submit');
    if (submitBtn) {
        submitBtn.disabled = true;
        submitBtn.textContent = 'Submitting...';
    }
    
    try {
        const url = currentRatingId 
            ? `/api/ratings/${currentRatingId}` 
            : '/api/ratings';
        
        const method = currentRatingId ? 'PUT' : 'POST';
        
        const body = currentRatingId 
            ? { score: currentScore }
            : { bookId: currentBookId, score: currentScore };
        
        const response = await fetch(url, {
            method: method,
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(body)
        });
        
        if (response.ok) {
            closeRatingModal();
            location.reload(); // Refresh to show updated rating
        } else {
            const error = await response.text();
            alert('Error submitting rating: ' + error);
        }
    } catch (error) {
        console.error('Error:', error);
        alert('Error submitting rating. Please try again.');
    } finally {
        if (submitBtn) {
            submitBtn.disabled = false;
            submitBtn.textContent = 'Submit Rating';
        }
    }
}

async function deleteRating(ratingId) {
    if (!confirm('Are you sure you want to delete this rating?')) {
        return;
    }
    
    try {
        const response = await fetch(`/api/ratings/${ratingId}`, {
            method: 'DELETE'
        });
        
        if (response.ok) {
            location.reload();
        } else {
            alert('Error deleting rating');
        }
    } catch (error) {
        console.error('Error:', error);
        alert('Error deleting rating. Please try again.');
    }
}

// Initialize rating displays on page load
document.addEventListener('DOMContentLoaded', function() {
    // Display static ratings
    document.querySelectorAll('[data-rating]').forEach(element => {
        const rating = parseFloat(element.getAttribute('data-rating'));
        const starsContainer = element.querySelector('.stars');
        if (starsContainer) {
            starsContainer.innerHTML = displayStarRating(rating);
        }
    });
    
    // Close modal when clicking outside
    const modal = document.getElementById('ratingModal');
    if (modal) {
        modal.addEventListener('click', (e) => {
            if (e.target === modal) {
                closeRatingModal();
            }
        });
    }
});
