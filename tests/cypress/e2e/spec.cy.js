describe("Playlist test", () => {
  it("able to create and delete a playlist", function () {
    cy.visit("http://localhost:5173/main");
    cy.get('[data-testid="new-playlist-btn"]').click();
    cy.get('[data-testid="playlist-modal"] [name="name"]').type("test");
    cy.get('[data-testid="playlist-modal"] [name="hostId"]').clear();
    cy.get('[data-testid="playlist-modal"] [name="hostId"]').type("6");
    cy.get('[data-testid="playlist-modal"] [name="description"]').type("test");
    cy.get(
      '[data-testid="playlist-modal"] div.border button.text-white.w-full'
    ).click();
    cy.get(".playlist-card").last().should("be.visible");
    cy.getId("playlist-list__name").contains("test").should("be.visible");
    cy.get(".playlist-card")
      .contains("test")
      .parents(".playlist-card")
      .find('[data-testid="playlist-list__delete-icon"]')
      .click();
  });
});
