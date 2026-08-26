# Facebook Upload Feature - Manual Testing Guide

## Prerequisites for Testing
Before testing, you need to configure Facebook integration:

1. **Create a Facebook App** (if not already done):
   - Go to https://developers.facebook.com/
   - Create a new app or use existing
   - Add "Pages" permission

2. **Generate Page Access Token**:
   - In Facebook App, go to Tools → Graph API Explorer
   - Select your Facebook page
   - Add permissions: `pages_manage_posts`, `pages_read_engagement`
   - Generate token and copy it

3. **Configure the application**:
   ```bash
   cd ArvidsonFoto
   dotnet user-secrets set "Facebook:PageAccessToken" "YOUR_TOKEN_HERE"
   dotnet user-secrets set "Facebook:PageId" "YOUR_PAGE_ID_HERE"
   ```

## Testing Steps

### 1. Start the Application
```bash
cd /home/runner/work/ArvidsonFoto-MVC-NET-web/ArvidsonFoto-MVC-NET-web/ArvidsonFoto
dotnet run
```

### 2. Login as Administrator
- Navigate to the login page
- Log in with administrator credentials

### 3. Access Facebook Upload Page
- From UploadAdmin, click on "Dela på Facebook" in the navigation menu
- You should see a page with:
  - A text area for writing a message
  - A grid of recently uploaded images (up to 25)
  - Each image should be clickable to select/deselect

### 4. Test Image Selection
- **Test 1**: Click on images to select them
  - Selected images should have a blue border
  - The counter badge should update (e.g., "3 valda")
  - Checkboxes should be checked

- **Test 2**: Try selecting more than 10 images
  - After selecting 10, remaining images should be disabled
  - Counter should show warning (red badge)
  - Submit button should be disabled

- **Test 3**: Deselect images
  - Click on selected images to deselect
  - Border should disappear
  - Counter should update
  - Previously disabled images should become available

### 5. Test Form Validation
- **Test 1**: Try submitting without selecting images
  - Should show alert: "Du måste välja minst en bild!"

- **Test 2**: Try submitting without message
  - Should show alert: "Du måste skriva ett meddelande!"

- **Test 3**: Try submitting with valid data
  - Select 1-5 images
  - Write a message (e.g., "Test från hemsidan!")
  - Click "Publicera på Facebook"
  - Button should show loading spinner
  - After success, you should see a green success message
  - A link to the Facebook post should be displayed

### 6. Test Without Configuration
If you want to test the "not configured" scenario:
- Remove the Facebook configuration temporarily
- Try to submit
- Should see warning: "Facebook är inte konfigurerat"

### 7. Verify on Facebook
- Click the link to the Facebook post
- Verify that:
  - The post appears on your Facebook page
  - All selected images are included
  - The message text is correct

## Expected UI Elements

### Navigation
- "Dela på Facebook" link in UploadAdmin menu with Facebook icon (bi-facebook)

### Page Layout
- Breadcrumb: ArvidsonFoto.se > Upload Admin > Dela bilder på Facebook
- Left sidebar with UploadAdmin navigation
- Main content area with:
  - Message textarea (required, max 5000 characters)
  - Selected count badge (updates dynamically)
  - Image grid (responsive: 2 cols on mobile, 3 on tablet, 4 on desktop)
  - Submit button with Facebook icon
  - Cancel button linking back to UploadAdmin

### Success State
- Green alert box with:
  - Success icon (bi-check-circle)
  - Text: "Inlägget har publicerats på Facebook!"
  - Button to view the post on Facebook

### Error States
- Red alert for Facebook API errors
- Yellow alert for configuration issues
- Yellow alert for validation errors

## Testing Different Scenarios

### Scenario 1: Single Image Upload
1. Select exactly 1 image
2. Write a message
3. Submit
4. Verify single image appears in Facebook post

### Scenario 2: Multiple Images Upload
1. Select 5-10 images
2. Write a message
3. Submit
4. Verify all images appear in Facebook post (as an album/carousel)

### Scenario 3: Network Error Simulation
To test error handling, you could temporarily:
- Use an invalid token
- Use an invalid page ID
- Check that appropriate error messages appear

## Common Issues and Solutions

### Issue: "Facebook är inte konfigurerat"
- **Solution**: Verify Facebook:PageAccessToken and Facebook:PageId are set in user secrets

### Issue: "Fel vid publicering"
- **Possible causes**:
  - Invalid or expired token
  - Missing permissions
  - Network issues
- **Solution**: Check logs for detailed error message, regenerate token if needed

### Issue: Images not loading in the grid
- **Possible causes**:
  - No images in database
  - Image URLs not accessible
- **Solution**: Upload some images first via "Ny bild" page

## Clean Up After Testing
After testing, you may want to:
- Delete test posts from Facebook page
- Keep or remove the Facebook configuration from user secrets

## Notes for Developer
- All 84 tests pass ✅
- CodeQL security scan: 0 vulnerabilities ✅
- Build successful with only existing warnings ✅
- Ready for production deployment after configuration
