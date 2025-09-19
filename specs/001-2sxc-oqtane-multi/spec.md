# Feature Specification: 2sxc Oqtane Multi-Database (Multi-Tenant) Support

**Feature Branch**: `001-2sxc-oqtane-multi`  
**Created**: 2025-09-19  
**Status**: Draft  
**Input**: User description: "Enable 2sxc Oqtane to support Oqtane multi-tenant with per-tenant databases; select the correct DB per request based on current Oqtane Tenant + Site; treat identity as composite {TenantId, SiteId}; tenant-scoped caching/APIs/logging; persist TenantId in EAV rows and TsDynDataZone; no feature flag; coordinate any required EAV updates with this spec."

## Execution Flow (main)
```
1. Parse user description from Input
   → If empty: ERROR "No feature description provided"
2. Extract key concepts from description
   → Identify: actors (admin/editors), actions (create/edit content/apps), data (tenant DBs), constraints (isolation, backward compatibility)
3. For each unclear aspect:
   → Mark with [NEEDS CLARIFICATION: specific question]
4. Fill User Scenarios & Testing section
## Execution Flow (main)

```text
5. Generate Functional Requirements
7. Run Review Checklist
   → If any [NEEDS CLARIFICATION]: WARN "Spec has uncertainties"

---

## ⚡ Quick Guidelines
- ✅ Focus on WHAT users need and WHY
- ❌ Avoid HOW to implement (no tech stack, APIs, code structure)
- 👥 Written for business stakeholders, not developers

### Section Requirements
- **Mandatory sections**: Must be completed for every feature
- **Optional sections**: Include only when relevant to the feature
- When a section doesn't apply, remove it entirely (don't leave as "N/A")

### For AI Generation
When creating this spec from a user prompt:
1. **Mark all ambiguities**: Use [NEEDS CLARIFICATION: specific question] for any assumption you'd need to make
2. **Don't guess**: If the prompt doesn't specify something (e.g., "login system" without auth method), mark it
3. **Think like a tester**: Every vague requirement should fail the "testable and unambiguous" checklist item
4. **Common underspecified areas**:
   - User types and permissions
   - Data retention/deletion policies  
   - Performance targets and scale
   - Error handling behaviors
   - Integration requirements
   - Security/compliance needs

---
As an Oqtane site administrator or editor, I want 2sxc apps and content on my site to use only my tenant’s data so that content remains isolated between tenants even when SiteId values are the same in different databases.

### Acceptance Scenarios
1. Given two Oqtane tenants (each with its own database), when a user creates content in Tenant A, then the content does not appear in Tenant B, and queries only read/write Tenant A’s database.
- Tenant or site renames/alias changes must not break tenant scoping of data access.

- **FR-002**: The system MUST select the correct tenant database based on the current Tenant.
- **FR-003**: The system MUST treat site identity as composite {TenantId, SiteId} across all 2sxc operations.
- **FR-008**: The system MUST persist TenantId in 2sxc/EAV data records (including TsDynDataZone) where required to ensure isolation.
- **FR-009**: The system MUST remain backward compatible for single-tenant (Master-only) environments.

### Non-Functional Requirements
- **NFR-001**: Tenant/Site resolution and database selection SHOULD add ≤ 1 ms overhead after warm-up.
### Key Entities (include if feature involves data)
- **Tenant**: An Oqtane tenant, backed by a dedicated database; parent of Site.

## Review & Acceptance Checklist
*GATE: Automated checks run during main() execution*

- [ ] No [NEEDS CLARIFICATION] markers remain
- [ ] Success criteria are measurable
- [ ] Scope is clearly bounded
- [ ] Dependencies and assumptions identified

---

## Execution Status
*Updated by main() during processing*

- [ ] User description parsed
- [ ] Key concepts extracted
- [ ] Ambiguities marked
- [ ] User scenarios defined
- [ ] Requirements generated
- [ ] Entities identified
- [ ] Review checklist passed

---
